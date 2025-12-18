using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    /// <summary>
    /// ❌ Chấm công theo username
    //❌ Cho sửa giờ vào/ra bằng UI
    //❌ Cho check-in nhiều lần
    //❌ Nghỉ phép vẫn check-in
    //❌ Không gắn login với nhân viên
    /// <summary>
    /// Mỗi nhân viên – mỗi ngày – tối đa 1 bản ghi
    //Check-in = tạo bản ghi
    //Check-out = cập nhật thoiGianRa
    //Nghỉ phép = tạo bản ghi với:
    //nghiPhep = true
    //thoiGianVao = ngày nghỉ + 00:00
    //Mọi kiểm tra theo thoiGianVao.Date
    /// </summary>
    public class ChamCongService : IChamCongService
    {
        private readonly ApplicationDbContext _context;

        public ChamCongService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. CHECK-IN
        public async Task CheckInAsync(int nhanVienId, bool anTrua)
        {
            var today = DateTime.Today;

            var tonTai = await _context.ChamCongs.AnyAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == today
            );

            if (tonTai)
                throw new Exception("Da cham cong hom nay");

            var chamCong = new ChamCong
            {
                nhanVienId = nhanVienId,
                thoiGianVao = DateTime.Now,
                anTrua = anTrua,
                nghiPhep = false,
                nghiPhepCoLuong = false
            };

            _context.ChamCongs.Add(chamCong);
            await _context.SaveChangesAsync();
        }

        // 2. CHECK-OUT
        public async Task CheckOutAsync(int nhanVienId)
        {
            var today = DateTime.Today;

            var chamCong = await _context.ChamCongs.FirstOrDefaultAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == today &&
                !x.nghiPhep
            );

            if (chamCong == null)
                throw new Exception("Chua check-in hom nay");

            if (chamCong.thoiGianRa.HasValue)
                throw new Exception("Da check-out");

            chamCong.thoiGianRa = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        // 3. ĐĂNG KÝ NGHỈ PHÉP
        public async Task DangKyNghiPhepAsync(int nhanVienId, DateTime ngay, bool coLuong)
        {
            ngay = ngay.Date;

            var tonTai = await _context.ChamCongs.AnyAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == ngay
            );

            if (tonTai)
                throw new Exception("Ngay nay da co cham cong");

            var chamCong = new ChamCong
            {
                nhanVienId = nhanVienId,
                thoiGianVao = ngay, // 00:00
                nghiPhep = true,
                nghiPhepCoLuong = coLuong,
                anTrua = false
            };

            _context.ChamCongs.Add(chamCong);
            await _context.SaveChangesAsync();
        }

        // 4. LẤY CHẤM CÔNG HÔM NAY
        public async Task<ChamCong?> LayChamCongHomNayAsync(int nhanVienId)
        {
            var today = DateTime.Today;

            return await _context.ChamCongs.FirstOrDefaultAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == today
            );
        }

        // 5. LẤY CHẤM CÔNG THEO THÁNG
        public async Task<List<ChamCong>> LayChamCongTheoThangAsync(
            int nhanVienId, int thang, int nam)
        {
            return await _context.ChamCongs
                .Where(x =>
                    x.nhanVienId == nhanVienId &&
                    x.thoiGianVao.Month == thang &&
                    x.thoiGianVao.Year == nam
                )
                .OrderBy(x => x.thoiGianVao)
                .ToListAsync();
        }
    }

    public interface IChamCongService
    {
        Task CheckInAsync(int nhanVienId, bool anTrua);
        Task CheckOutAsync(int nhanVienId);
        Task DangKyNghiPhepAsync(int nhanVienId, DateTime ngay, bool coLuong);

        Task<ChamCong?> LayChamCongHomNayAsync(int nhanVienId);
        Task<List<ChamCong>> LayChamCongTheoThangAsync(int nhanVienId, int thang, int nam);
    }

}
