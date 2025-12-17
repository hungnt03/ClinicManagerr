using ClinicManager.Data;
using ClinicManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    /// <summary>
    /// ❌ Trừ buổi khi hoàn thành → sai (dễ gian lận)
    //❌ Cho sửa buổi đã hoàn thành
    //❌ Không khóa gói khi hết buổi
    //❌ Cho tạo nhiều buổi 1 ngày
    //❌ Không dùng transaction
    /// </summary>
    public class BuoiDieuTriService : IBuoiDieuTriService
    {
        private readonly ApplicationDbContext _context;

        public BuoiDieuTriService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Tạo buổi điều trị – ÉP NGHIỆP VỤ
        public async Task<BuoiDieuTri> TaoBuoiDieuTriAsync(
            int benhNhanId,
            int benhNhanGoiDieuTriId,
            DateTime ngayDieuTri,
            TimeSpan gioBatDau,
            TimeSpan gioKetThuc,
            string chiDinhDacBiet
        )
        {
            var goi = await _context.BenhNhanGoiDieuTris
                .FirstOrDefaultAsync(x => x.benhNhanGoiDieuTriId == benhNhanGoiDieuTriId);

            if (goi == null)
                throw new Exception("Khong tim thay goi dieu tri");

            if (goi.trangThai != "DangSuDung")
                throw new Exception("Goi dieu tri khong con su dung");

            if (goi.soBuoiDaDung >= goi.tongSoBuoi)
                throw new Exception("Goi dieu tri da het buoi");

            // Không cho 1 ngày điều trị 2 buổi
            var daCoBuoi = await _context.BuoiDieuTris.AnyAsync(x =>
                x.benhNhanId == benhNhanId &&
                x.ngayDieuTri.Date == ngayDieuTri.Date &&
                x.trangThai != "Huy"
            );

            if (daCoBuoi)
                throw new Exception("Benh nhan da dieu tri trong ngay");

            using var tran = await _context.Database.BeginTransactionAsync();

            try
            {
                var buoi = new BuoiDieuTri
                {
                    benhNhanId = benhNhanId,
                    benhNhanGoiDieuTriId = benhNhanGoiDieuTriId,
                    ngayDieuTri = ngayDieuTri.Date,
                    gioBatDau = gioBatDau,
                    gioKetThuc = gioKetThuc,
                    chiDinhDacBiet = chiDinhDacBiet,
                    trangThai = "DangThucHien"
                };

                _context.BuoiDieuTris.Add(buoi);
                await _context.SaveChangesAsync();

                // Trừ buổi
                goi.soBuoiDaDung += 1;

                if (goi.soBuoiDaDung >= goi.tongSoBuoi)
                    goi.trangThai = "HetBuoi";

                _context.BenhNhanGoiDieuTris.Update(goi);
                await _context.SaveChangesAsync();

                await tran.CommitAsync();
                return buoi;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        // 2. Thêm nhân viên vào buổi
        public async Task ThemNhanVienAsync(
            int buoiDieuTriId,
            int nhanVienId,
            string vaiTro
        )
        {
            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == buoiDieuTriId);

            if (buoi == null)
                throw new Exception("Buoi dieu tri khong ton tai");

            if (buoi.trangThai == "HoanThanh")
                throw new Exception("Khong the sua buoi da hoan thanh");

            var tonTai = await _context.BuoiDieuTriNhanViens.AnyAsync(x =>
                x.buoiDieuTriId == buoiDieuTriId &&
                x.nhanVienId == nhanVienId &&
                x.vaiTro == vaiTro
            );

            if (tonTai)
                return;

            _context.BuoiDieuTriNhanViens.Add(new BuoiDieuTriNhanVien
            {
                buoiDieuTriId = buoiDieuTriId,
                nhanVienId = nhanVienId,
                vaiTro = vaiTro
            });

            await _context.SaveChangesAsync();
        }

        // 3. Thêm vật tư / thuốc
        public async Task ThemVatTuAsync(
            int buoiDieuTriId,
            int vatTuId,
            int soLuong
        )
        {
            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == buoiDieuTriId);

            if (buoi == null)
                throw new Exception("Buoi dieu tri khong ton tai");

            if (buoi.trangThai == "HoanThanh")
                throw new Exception("Khong the them vat tu cho buoi da hoan thanh");

            var vatTu = await _context.VatTus
                .FirstOrDefaultAsync(x => x.vatTuId == vatTuId);

            if (vatTu == null)
                throw new Exception("Vat tu khong ton tai");

            if (vatTu.tonKho < soLuong)
                throw new Exception("Khong du ton kho");

            vatTu.tonKho -= soLuong;

            _context.ThuocVatTuBuoiDieuTris.Add(new ThuocVatTuBuoiDieuTri
            {
                buoiDieuTriId = buoiDieuTriId,
                vatTuId = vatTuId,
                soLuong = soLuong,
                donGia = vatTu.donGia
            });

            await _context.SaveChangesAsync();
        }

        // 4. Hoàn thành buổi điều trị
        public async Task HoanThanhBuoiAsync(int buoiDieuTriId)
        {
            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == buoiDieuTriId);

            if (buoi == null)
                throw new Exception("Buoi dieu tri khong ton tai");

            if (buoi.trangThai == "HoanThanh")
                return;

            buoi.trangThai = "HoanThanh";
            await _context.SaveChangesAsync();
        }
    }

    public interface IBuoiDieuTriService
    {
        Task<BuoiDieuTri> TaoBuoiDieuTriAsync(
            int benhNhanId,
            int benhNhanGoiDieuTriId,
            DateTime ngayDieuTri,
            TimeSpan gioBatDau,
            TimeSpan gioKetThuc,
            string chiDinhDacBiet
        );

        Task ThemNhanVienAsync(
            int buoiDieuTriId,
            int nhanVienId,
            string vaiTro
        );

        Task ThemVatTuAsync(
            int buoiDieuTriId,
            int vatTuId,
            int soLuong
        );

        Task HoanThanhBuoiAsync(int buoiDieuTriId);
    }
}
