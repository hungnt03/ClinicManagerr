using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.Services.Luong;
using ClinicManager.ViewModels.BuoiDieuTri;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    /// <summary>
    //✔ Không tạo buổi nếu đợt đã HoanThanh
    //✔ Không tạo buổi nếu đã hết buổi
    //✔ Không dùng vật tư nếu không đủ tồn kho
    //✔ Snapshot giá tại thời điểm dùng
    //✔ Không cho sửa chiPhiThuocVatTu bằng tay
    //✔ Mọi thay đổi vật tư đều cập nhật tồn kho
    /// </summary>
    public class BuoiDieuTriService : IBuoiDieuTriService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDotDieuTriService _dotDieuTriService;

        public BuoiDieuTriService(
            ApplicationDbContext context,
            IDotDieuTriService dotDieuTriService)
        {
            _context = context;
            _dotDieuTriService = dotDieuTriService;
        }

        // ==================================================
        // 1. TẠO BUỔI ĐIỀU TRỊ
        // ==================================================
        public async Task<int> TaoBuoiDieuTriAsync(
            int dotDieuTriId,
            int benhNhanId,
            DateTime ngayDieuTri,
            int? bacSiDieuTriTayId,
            int? kyThuatVienTapId,
            string noiDungTap,
            string noiDungDieuTriTay,
            string chiDinhDacBiet)
        {
            if (await LuongLockHelper.DaChotLuongAsync(_context, ngayDieuTri))
            {
                throw new Exception("Thang nay da chot luong, khong duoc sua buoi dieu tri");
            }

            var dot = await _context.DotDieuTris
                .FirstOrDefaultAsync(x => x.dotDieuTriId == dotDieuTriId);

            if (dot == null)
                throw new Exception("Khong tim thay dot dieu tri");

            // ❗ ép nghiệp vụ
            if (dot.trangThai == TrangThaiDotDieuTri.HoanThanh)
                throw new Exception("Dot dieu tri da hoan thanh");

            if (dot.soBuoiDaDung >= dot.tongSoBuoi)
                throw new Exception("Da het so buoi dieu tri");

            var buoi = new BuoiDieuTri
            {
                dotDieuTriId = dotDieuTriId,
                benhNhanId = benhNhanId,
                ngayDieuTri = ngayDieuTri,

                bacSiDieuTriTayId = bacSiDieuTriTayId,
                kyThuatVienTapId = kyThuatVienTapId,

                noiDungTap = noiDungTap,
                noiDungDieuTriTay = noiDungDieuTriTay,
                chiDinhDacBiet = chiDinhDacBiet,

                chiPhiThuocVatTu = 0,
                taoLuc = DateTime.Now
            };

            _context.BuoiDieuTris.Add(buoi);
            await _context.SaveChangesAsync();

            // ❗ trừ buổi + cập nhật trạng thái đợt
            await _dotDieuTriService.TangSoBuoiDaDungAsync(dotDieuTriId);

            return buoi.buoiDieuTriId;
        }

        // ==================================================
        // 2. THÊM VẬT TƯ / THUỐC CHO BUỔI
        // ==================================================
        public async Task ThemVatTuAsync(
            int buoiDieuTriId,
            int vatTuId,
            int soLuong)
        {
            if (soLuong <= 0)
                throw new Exception("So luong phai > 0");

            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == buoiDieuTriId);

            if (buoi == null)
                throw new Exception("Khong tim thay buoi dieu tri");

            var vatTu = await _context.VatTus.FirstOrDefaultAsync(x => x.vatTuId == vatTuId);

            if (vatTu == null)
                throw new Exception("Vat tu khong ton tai");

            if (vatTu.tonKho < soLuong)
                throw new Exception("Ton kho khong du");

            // trừ tồn kho
            vatTu.tonKho -= soLuong;

            var dong = new ThuocVatTuBuoiDieuTri
            {
                buoiDieuTriId = buoiDieuTriId,
                vatTuId = vatTuId,
                soLuong = soLuong,
                donGia = vatTu.donGia
            };

            _context.ThuocVatTuBuoiDieuTris.Add(dong);

            // cập nhật tổng chi phí
            buoi.chiPhiThuocVatTu += soLuong * vatTu.donGia;

            await _context.SaveChangesAsync();
        }

        // ==================================================
        // 3. SỬA SỐ LƯỢNG VẬT TƯ
        // ==================================================
        public async Task SuaVatTuAsync(
            int thuocVatTuBuoiDieuTriId,
            int soLuongMoi)
        {
            if (soLuongMoi <= 0)
                throw new Exception("So luong moi phai > 0");

            var dong = await _context.ThuocVatTuBuoiDieuTris
                .FirstOrDefaultAsync(x => x.id == thuocVatTuBuoiDieuTriId);

            if (dong == null)
                throw new Exception("Khong tim thay dong vat tu");

            var vatTu = await _context.VatTus
                .FirstOrDefaultAsync(x => x.vatTuId == dong.vatTuId);

            if (vatTu == null)
                throw new Exception("Vat tu khong ton tai");

            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == dong.buoiDieuTriId);

            if (buoi == null)
                throw new Exception("Khong tim thay buoi dieu tri");

            var chenhlech = soLuongMoi - dong.soLuong;

            // kiểm tra tồn kho nếu tăng
            if (chenhlech > 0 && vatTu.tonKho < chenhlech)
                throw new Exception("Ton kho khong du");

            // cập nhật tồn kho
            vatTu.tonKho -= chenhlech;

            // cập nhật tổng chi phí
            buoi.chiPhiThuocVatTu += chenhlech * dong.donGia;

            dong.soLuong = soLuongMoi;

            await _context.SaveChangesAsync();
        }

        // ==================================================
        // 4. XOÁ VẬT TƯ KHỎI BUỔI
        // ==================================================
        public async Task XoaVatTuAsync(int thuocVatTuBuoiDieuTriId)
        {
            var dong = await _context.ThuocVatTuBuoiDieuTris
                .FirstOrDefaultAsync(x => x.id == thuocVatTuBuoiDieuTriId);

            if (dong == null)
                throw new Exception("Khong tim thay dong vat tu");

            var vatTu = await _context.VatTus
                .FirstOrDefaultAsync(x => x.vatTuId == dong.vatTuId);

            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == dong.buoiDieuTriId);

            if (vatTu != null)
            {
                // hoàn tồn kho
                vatTu.tonKho += dong.soLuong;
            }

            if (buoi != null)
            {
                // trừ chi phí snapshot
                buoi.chiPhiThuocVatTu -= dong.soLuong * dong.donGia;
            }

            _context.ThuocVatTuBuoiDieuTris.Remove(dong);
            await _context.SaveChangesAsync();
        }

        public async Task CapNhatBuoiDieuTriAsync(
    BuoiDieuTriEditVm vm,
    int adminNhanVienId)
        {
            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == vm.BuoiDieuTriId);

            if (await LuongLockHelper.DaChotLuongAsync(_context, buoi.ngayDieuTri))
            {
                throw new Exception("Thang nay da chot luong, khong duoc sua buoi dieu tri");
            }

            if (buoi == null)
                throw new Exception("Khong tim thay buoi dieu tri");

            // ===== LƯU AUDIT =====
            var audit = new BuoiDieuTriAudit
            {
                buoiDieuTriId = buoi.buoiDieuTriId,
                adminNhanVienId = adminNhanVienId,

                ngayDieuTriCu = buoi.ngayDieuTri,
                bacSiDieuTriTayIdCu = buoi.bacSiDieuTriTayId,
                kyThuatVienTapIdCu = buoi.kyThuatVienTapId,
                noiDungTapCu = buoi.noiDungTap,
                noiDungDieuTriTayCu = buoi.noiDungDieuTriTay,
                chiDinhDacBietCu = buoi.chiDinhDacBiet,
                chiPhiThuocVatTuCu = buoi.chiPhiThuocVatTu,

                ngayDieuTriMoi = vm.NgayDieuTri,
                bacSiDieuTriTayIdMoi = vm.BacSiDieuTriTayId,
                kyThuatVienTapIdMoi = vm.KyThuatVienTapId,
                noiDungTapMoi = vm.NoiDungTap,
                noiDungDieuTriTayMoi = vm.NoiDungDieuTriTay,
                chiDinhDacBietMoi = vm.ChiDinhDacBiet,
                chiPhiThuocVatTuMoi = buoi.chiPhiThuocVatTu,

                lyDo = vm.LyDoSua,
                suaLuc = DateTime.Now
            };

            _context.BuoiDieuTriAudits.Add(audit);

            // ===== UPDATE BUỔI =====
            buoi.ngayDieuTri = vm.NgayDieuTri;
            buoi.bacSiDieuTriTayId = vm.BacSiDieuTriTayId;
            buoi.kyThuatVienTapId = vm.KyThuatVienTapId;
            buoi.noiDungTap = vm.NoiDungTap;
            buoi.noiDungDieuTriTay = vm.NoiDungDieuTriTay;
            buoi.chiDinhDacBiet = vm.ChiDinhDacBiet;

            await _context.SaveChangesAsync();
        }

    }

    public interface IBuoiDieuTriService
    {
        // ===== TẠO BUỔI ĐIỀU TRỊ =====
        Task<int> TaoBuoiDieuTriAsync(
            int dotDieuTriId,
            int benhNhanId,
            DateTime ngayDieuTri,
            int? bacSiDieuTriTayId,
            int? kyThuatVienTapId,
            string noiDungTap,
            string noiDungDieuTriTay,
            string chiDinhDacBiet
        );

        // ===== THÊM VẬT TƯ / THUỐC =====
        Task ThemVatTuAsync(
            int buoiDieuTriId,
            int vatTuId,
            int soLuong
        );

        Task SuaVatTuAsync(
            int thuocVatTuBuoiDieuTriId,
            int soLuongMoi
        );

        Task XoaVatTuAsync(int thuocVatTuBuoiDieuTriId);

        Task CapNhatBuoiDieuTriAsync(
            BuoiDieuTriEditVm vm,
            int adminNhanVienId);
    }
}
