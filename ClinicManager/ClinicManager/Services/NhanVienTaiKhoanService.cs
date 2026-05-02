using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Models.Entities;
using ClinicManager.ViewModels.NhanVien;
using Microsoft.AspNetCore.Identity;

namespace ClinicManager.Services
{
    public class NhanVienTaiKhoanService : INhanVienTaiKhoanService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public NhanVienTaiKhoanService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<int> TaoNhanVienVaTaiKhoanAsync(TaoNhanVienTaiKhoanDto dto)
        {
            // 1. Check role hợp lệ
            if (!await _roleManager.RoleExistsAsync(dto.role))
                throw new Exception("Role không hợp lệ");

            // 2. Check email đã tồn tại
            if (await _userManager.FindByEmailAsync(dto.email) != null)
                throw new Exception("Email đã tồn tại");

            using var tran = await _context.Database.BeginTransactionAsync();

            try
            {
                // 3. Tạo nhân viên
                var nhanVien = new NhanVien
                {
                    hoTen = dto.hoTen,
                    vaiTro = dto.vaiTro,
                    luongCoBan = dto.luongCoBan,
                    hoatDong = true
                };

                _context.NhanViens.Add(nhanVien);
                await _context.SaveChangesAsync();

                // 4. Tạo user
                var user = new ApplicationUser
                {
                    UserName = dto.email,
                    Email = dto.email,
                    EmailConfirmed = true,
                    nhanVienId = nhanVien.nhanVienId
                };

                var result = await _userManager.CreateAsync(user, dto.matKhau);
                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                // 5. Gán role
                await _userManager.AddToRoleAsync(user, dto.role);

                await tran.CommitAsync();

                return nhanVien.nhanVienId;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }
    }

    public interface INhanVienTaiKhoanService
    {
        Task<int> TaoNhanVienVaTaiKhoanAsync(TaoNhanVienTaiKhoanDto dto);
    }
}
