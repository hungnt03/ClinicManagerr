using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.Services.Luong;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public interface IThanhToanService
    {
        Task<int> ThuTienGoiAsync(
            int dotDieuTriId,
            decimal soTien,
            HinhThucThanhToan hinhThuc,
            string? ghiChu,
            DateTime? ngayThu = null);

        Task<int> ThuTienThuocVatTuAsync(
            int buoiDieuTriId,
            decimal soTien,
            HinhThucThanhToan hinhThuc,
            string? ghiChu,
            DateTime? ngayThu = null);

        Task<List<ThanhToan>> GetByDotDieuTriAsync(int dotDieuTriId);
        Task<List<ThanhToan>> GetByBuoiDieuTriAsync(int buoiDieuTriId);
    }
    public class ThanhToanService : IThanhToanService
    {
        private readonly ApplicationDbContext _context;

        public ThanhToanService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================================================
        // 1. THU TIỀN GÓI ĐIỀU TRỊ
        // ==================================================
        public async Task<int> ThuTienGoiAsync(
            int dotDieuTriId,
            decimal soTien,
            HinhThucThanhToan hinhThuc,
            string? ghiChu,
            DateTime? ngayThu = null)
        {
            if (await LuongLockHelper.DaChotLuongAsync(_context, ngayThu ?? DateTime.Now))
            {
                throw new Exception("Tháng này đã chốt lương, không được thu tiền");
            }


            if (soTien <= 0)
                throw new Exception("Số tiền phải > 0");

            var dot = await _context.DotDieuTris
                .FirstOrDefaultAsync(x => x.dotDieuTriId == dotDieuTriId);

            if (dot == null)
                throw new Exception("Không tìm thấy đợt điều trị");

            if (dot.trangThai == TrangThaiDotDieuTri.HoanThanh)
                throw new Exception("Đợt điều trị đã hoàn thành");

            decimal conLai = dot.tongTien - dot.daThanhToan;

            if (soTien > conLai)
                throw new Exception($"Số tiền vượt quá số tiền còn lại ({conLai:N0})");

            var thanhToan = new ThanhToan
            {
                loai = LoaiThanhToan.GoiDieuTri,
                dotDieuTriId = dotDieuTriId,
                soTien = soTien,
                ngayThu = ngayThu ?? DateTime.Now,
                hinhThuc = hinhThuc,
                ghiChu = ghiChu,
                daChot = false,
                taoLuc = DateTime.Now
            };

            _context.ThanhToans.Add(thanhToan);

            // cập nhật đợt
            dot.daThanhToan += soTien;

            if (dot.daThanhToan >= dot.tongTien && dot.ngayThanhToan == null)
            {
                dot.ngayThanhToan = thanhToan.ngayThu;
            }

            await _context.SaveChangesAsync();
            return thanhToan.thanhToanId;
        }

        // ==================================================
        // 2. THU TIỀN THUỐC / VẬT TƯ
        // ==================================================
        public async Task<int> ThuTienThuocVatTuAsync(
            int buoiDieuTriId,
            decimal soTien,
            HinhThucThanhToan hinhThuc,
            string? ghiChu,
            DateTime? ngayThu = null)
        {
            if (await LuongLockHelper.DaChotLuongAsync(_context, ngayThu ?? DateTime.Now))
            {
                throw new Exception("Tháng này đã chốt lương, không được thu tiền");
            }


            if (soTien <= 0)
                throw new Exception("Số tiền phải > 0");

            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == buoiDieuTriId);

            if (buoi == null)
                throw new Exception("Không tìm thấy đợt điều trị");

            if (soTien > buoi.chiPhiThuocVatTu)
                throw new Exception("Số tiền thu vượt quá chi phí thuốc / vật tư");

            var thanhToan = new ThanhToan
            {
                loai = LoaiThanhToan.ThuocVatTu,
                buoiDieuTriId = buoiDieuTriId,
                soTien = soTien,
                ngayThu = ngayThu ?? DateTime.Now,
                hinhThuc = hinhThuc,
                ghiChu = ghiChu,
                daChot = false,
                taoLuc = DateTime.Now
            };

            _context.ThanhToans.Add(thanhToan);
            await _context.SaveChangesAsync();

            return thanhToan.thanhToanId;
        }

        // ==================================================
        // 3. TRA CỨU
        // ==================================================
        public async Task<List<ThanhToan>> GetByDotDieuTriAsync(int dotDieuTriId)
        {
            return await _context.ThanhToans
                .Where(x => x.dotDieuTriId == dotDieuTriId)
                .OrderBy(x => x.ngayThu)
                .ToListAsync();
        }

        public async Task<List<ThanhToan>> GetByBuoiDieuTriAsync(int buoiDieuTriId)
        {
            return await _context.ThanhToans
                .Where(x => x.buoiDieuTriId == buoiDieuTriId)
                .OrderBy(x => x.ngayThu)
                .ToListAsync();
        }
    }
}
