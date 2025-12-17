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
    /// </summary>
    public class ChamCongService : IChamCongService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChamCongService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lấy chấm công hôm nay theo user login
        public async Task<ChamCong?> LayChamCongHomNayAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.nhanVienId.HasValue)
                return null;

            var today = DateTime.Today;

            return await _context.ChamCongs.FirstOrDefaultAsync(x =>
                x.nhanVienId == user.nhanVienId.Value &&
                x.thoiGianVao.Date == today
            );
        }

        // CHECK-IN
        public async Task<ChamCong> CheckInAsync(string userId, bool anTrua)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.nhanVienId.HasValue)
                throw new Exception("Tai khoan chua gan nhan vien");

            var nhanVienId = user.nhanVienId.Value;
            var today = DateTime.Today;

            // Đã chấm công hôm nay chưa
            var daChamCong = await _context.ChamCongs.AnyAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == today
            );

            if (daChamCong)
                throw new Exception("Da check-in hom nay");

            // Kiểm tra nghỉ phép
            var nghiPhep = await _context.ChamCongs.AnyAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.nghiPhep &&
                x.thoiGianVao.Date == today
            );

            if (nghiPhep)
                throw new Exception("Hom nay dang nghi phep");

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

            return chamCong;
        }

        // CHECK-OUT
        public async Task<ChamCong> CheckOutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.nhanVienId.HasValue)
                throw new Exception("Tai khoan chua gan nhan vien");

            var nhanVienId = user.nhanVienId.Value;
            var today = DateTime.Today;

            var chamCong = await _context.ChamCongs.FirstOrDefaultAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == today
            );

            if (chamCong == null)
                throw new Exception("Chua check-in hom nay");

            if (chamCong.thoiGianRa.HasValue)
                throw new Exception("Da check-out");

            chamCong.thoiGianRa = DateTime.Now;
            await _context.SaveChangesAsync();

            return chamCong;
        }

        // Đăng ký nghỉ phép (Admin / HR)
        public async Task DangKyNghiPhepAsync(
            int nhanVienId,
            DateTime ngay,
            bool nghiPhepCoLuong)
        {
            var date = ngay.Date;

            var tonTai = await _context.ChamCongs.AnyAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == date
            );

            if (tonTai)
                throw new Exception("Da co cham cong trong ngay");

            var chamCong = new ChamCong
            {
                nhanVienId = nhanVienId,
                thoiGianVao = date,
                nghiPhep = true,
                nghiPhepCoLuong = nghiPhepCoLuong,
                anTrua = false
            };

            _context.ChamCongs.Add(chamCong);
            await _context.SaveChangesAsync();
        }
    }

    public interface IChamCongService
    {
        Task<ChamCong> CheckInAsync(string userId, bool anTrua);
        Task<ChamCong> CheckOutAsync(string userId);
        Task<ChamCong?> LayChamCongHomNayAsync(string userId);
        Task DangKyNghiPhepAsync(
            int nhanVienId,
            DateTime ngay,
            bool nghiPhepCoLuong
        );
    }
}
