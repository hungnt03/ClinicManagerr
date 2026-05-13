using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TaiKhoanService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task KhoaTaiKhoanAsync(int nhanVienId, string adminUserId)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.nhanVienId == nhanVienId);

            if (user == null)
                throw new Exception("Nhân viên chưa có tài khoản");

            // Không cho admin tự khóa mình
            if (user.Id == adminUserId)
                throw new Exception("Không thể khóa chính tài khoản đang đăng nhập");

            // Khóa vĩnh viễn (cho tới khi admin mở)
            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);

            var nhanVien = await _context.NhanViens.FindAsync(nhanVienId);
            nhanVien.hoatDong = false;
            await _context.SaveChangesAsync();
        }

        public async Task MoKhoaTaiKhoanAsync(int nhanVienId)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.nhanVienId == nhanVienId);

            if (user == null)
                throw new Exception("Nhân viên chưa có tài khoản");

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            var nhanVien = await _context.NhanViens.FindAsync(nhanVienId);
            nhanVien.hoatDong = true;
            await _context.SaveChangesAsync();
        }
    }
    public interface ITaiKhoanService
    {
        Task KhoaTaiKhoanAsync(int nhanVienId, string adminUserId);
        Task MoKhoaTaiKhoanAsync(int nhanVienId);
    }

}
