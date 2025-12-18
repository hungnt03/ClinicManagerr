using ClinicManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class ResetMatKhauService : IResetMatKhauService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetMatKhauService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ResetAsync(int nhanVienId, string matKhauMoi)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.nhanVienId == nhanVienId);

            if (user == null)
                throw new Exception("Nhan vien chua co tai khoan");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                matKhauMoi);

            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);
        }
    }
    public interface IResetMatKhauService
    {
        Task ResetAsync(int nhanVienId, string matKhauMoi);
    }

}
