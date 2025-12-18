using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class NhanVienUpdateService : INhanVienUpdateService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NhanVienUpdateService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CapNhatNhanVienVaRoleAsync(
            int nhanVienId,
            string hoTen,
            string vaiTroNhanVien,
            decimal luongCoBan,
            bool hoatDong,
            string roleDangNhap)
        {
            using var tran = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Cập nhật nhân viên
                var nv = await _context.NhanViens
                    .FirstOrDefaultAsync(x => x.nhanVienId == nhanVienId);

                if (nv == null)
                    throw new Exception("Nhan vien khong ton tai");

                nv.hoTen = hoTen;
                nv.vaiTro = vaiTroNhanVien;
                nv.luongCoBan = luongCoBan;
                nv.hoatDong = hoatDong;

                await _context.SaveChangesAsync();

                // 2. Cập nhật role đăng nhập (nếu có user)
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(x => x.nhanVienId == nhanVienId);

                if (user != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);

                    // chỉ cho 1 role chính
                    foreach (var r in currentRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(user, r);
                    }

                    await _userManager.AddToRoleAsync(user, roleDangNhap);
                }

                await tran.CommitAsync();
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }
    }
    public interface INhanVienUpdateService
    {
        Task CapNhatNhanVienVaRoleAsync(
            int nhanVienId,
            string hoTen,
            string vaiTroNhanVien,
            decimal luongCoBan,
            bool hoatDong,
            string roleDangNhap
        );
    }
}
