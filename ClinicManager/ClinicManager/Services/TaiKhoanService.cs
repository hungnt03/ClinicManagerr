using ClinicManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TaiKhoanService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task KhoaTaiKhoanAsync(int nhanVienId, string adminUserId)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.nhanVienId == nhanVienId);

            if (user == null)
                throw new Exception("Nhan vien chua co tai khoan");

            // Không cho admin tự khóa mình
            if (user.Id == adminUserId)
                throw new Exception("Khong the khoa chinh tai khoan dang dang nhap");

            // Khóa vĩnh viễn (cho tới khi admin mở)
            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);
        }

        public async Task MoKhoaTaiKhoanAsync(int nhanVienId)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.nhanVienId == nhanVienId);

            if (user == null)
                throw new Exception("Nhan vien chua co tai khoan");

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);
        }
    }
    public interface ITaiKhoanService
    {
        Task KhoaTaiKhoanAsync(int nhanVienId, string adminUserId);
        Task MoKhoaTaiKhoanAsync(int nhanVienId);
    }

}
