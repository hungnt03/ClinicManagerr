using ClinicManager.Data;
using ClinicManager.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class DotDieuTriService : IDotDieuTriService
    {
        private readonly ApplicationDbContext _context;

        public DotDieuTriService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================================================
        // 1. KHÁM – TẠO ĐỢT ĐIỀU TRỊ
        // ==================================================
        public async Task<int> TaoDotDieuTriAsync(
            int benhNhanId,
            int bacSiKhamId,
            DateTime ngayKham,
            string chanDoan,
            string phacDoDieuTri,
            int goiDieuTriId,
            decimal daThanhToan)
        {
            // ❗ Không cho tạo đợt mới nếu còn đợt đang điều trị
            var dotDangMo = await _context.DotDieuTris.AnyAsync(x =>
                x.benhNhanId == benhNhanId &&
                x.trangThai == TrangThaiDotDieuTri.DangDieuTri);

            if (dotDangMo)
                throw new Exception("Benh nhan dang co dot dieu tri chua hoan thanh");

            var goi = await _context.GoiDieuTris
                .FirstOrDefaultAsync(x => x.goiDieuTriId == goiDieuTriId && x.hoatDong);

            if (goi == null)
                throw new Exception("Goi dieu tri khong hop le");

            var dot = new DotDieuTri
            {
                benhNhanId = benhNhanId,
                bacSiKhamId = bacSiKhamId,
                ngayKham = ngayKham,
                chanDoan = chanDoan,
                phacDoDieuTri = phacDoDieuTri,

                goiDieuTriId = goi.goiDieuTriId,
                tongSoBuoi = goi.soBuoi,
                soBuoiDaDung = 0,

                tongTien = goi.gia,
                daThanhToan = daThanhToan,

                trangThai = TrangThaiDotDieuTri.MoiTao,
                taoLuc = DateTime.Now
            };

            _context.DotDieuTris.Add(dot);
            await _context.SaveChangesAsync();

            return dot.dotDieuTriId;
        }

        // ==================================================
        // 2. LẤY ĐỢT ĐANG ĐIỀU TRỊ CỦA BỆNH NHÂN
        // ==================================================
        public async Task<DotDieuTri?> LayDotDangDieuTriAsync(int benhNhanId)
        {
            return await _context.DotDieuTris
                .Where(x =>
                    x.benhNhanId == benhNhanId &&
                    (x.trangThai == TrangThaiDotDieuTri.MoiTao ||
                     x.trangThai == TrangThaiDotDieuTri.DangDieuTri))
                .OrderByDescending(x => x.taoLuc)
                .FirstOrDefaultAsync();
        }

        // ==================================================
        // 3. MUA THÊM BUỔI (KHÔNG TẠO ĐỢT MỚI)
        // ==================================================
        public async Task MuaThemBuoiAsync(
            int dotDieuTriId,
            int soBuoiThem,
            decimal soTien)
        {
            if (soBuoiThem <= 0)
                throw new Exception("So buoi them phai > 0");

            var dot = await _context.DotDieuTris
                .FirstOrDefaultAsync(x => x.dotDieuTriId == dotDieuTriId);

            if (dot == null)
                throw new Exception("Khong tim thay dot dieu tri");

            dot.tongSoBuoi += soBuoiThem;
            dot.tongTien += soTien;

            // nếu đã hoàn thành mà mua thêm → mở lại
            if (dot.trangThai == TrangThaiDotDieuTri.HoanThanh)
            {
                dot.trangThai = TrangThaiDotDieuTri.DangDieuTri;
            }

            _context.DotDieuTriMuaThems.Add(new DotDieuTriMuaThem
            {
                dotDieuTriId = dotDieuTriId,
                soBuoiThem = soBuoiThem,
                soTien = soTien,
                muaLuc = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        // ==================================================
        // 4. TĂNG SỐ BUỔI ĐÃ DÙNG (SAU KHI TẠO BUỔI)
        // ==================================================
        public async Task TangSoBuoiDaDungAsync(int dotDieuTriId)
        {
            var dot = await _context.DotDieuTris
                .FirstOrDefaultAsync(x => x.dotDieuTriId == dotDieuTriId);

            if (dot == null)
                throw new Exception("Khong tim thay dot dieu tri");

            if (dot.trangThai == TrangThaiDotDieuTri.HoanThanh)
                throw new Exception("Dot dieu tri da hoan thanh");

            if (dot.soBuoiDaDung >= dot.tongSoBuoi)
                throw new Exception("Da het so buoi dieu tri");

            dot.soBuoiDaDung += 1;

            // chuyển trạng thái
            if (dot.soBuoiDaDung == 1)
            {
                dot.trangThai = TrangThaiDotDieuTri.DangDieuTri;
            }

            if (dot.soBuoiDaDung == dot.tongSoBuoi)
            {
                dot.trangThai = TrangThaiDotDieuTri.HoanThanh;
            }

            await _context.SaveChangesAsync();
        }
    }

    public interface IDotDieuTriService
    {
        // ===== KHÁM – TẠO ĐỢT =====
        Task<int> TaoDotDieuTriAsync(
            int benhNhanId,
            int bacSiKhamId,
            DateTime ngayKham,
            string chanDoan,
            string phacDoDieuTri,
            int goiDieuTriId,
            decimal daThanhToan
        );

        // ===== LẤY ĐỢT ĐANG ĐIỀU TRỊ =====
        Task<DotDieuTri?> LayDotDangDieuTriAsync(int benhNhanId);

        // ===== MUA THÊM BUỔI =====
        Task MuaThemBuoiAsync(
            int dotDieuTriId,
            int soBuoiThem,
            decimal soTien
        );

        // ===== CẬP NHẬT TRẠNG THÁI SAU MỖI BUỔI =====
        Task TangSoBuoiDaDungAsync(int dotDieuTriId);
    }
}
