using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Services;
using ClinicManager.ViewModels;
using ClinicManager.ViewModels.NhanVien;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Security.Principal;

namespace ClinicManager.Controllers
{
    /// <summary>
    /// ✔ Nhân viên và tài khoản tạo trong 1 transaction
    //✔ Không có user mồ côi
    //✔ Role Identity ≠ Vai trò nghiệp vụ
    //✔ Backend ép logic, UI chỉ nhập
    //✔ Sau này gắn lương, chấm công, điều trị không lệch
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class NhanVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INhanVienTaiKhoanService _service;
        private readonly INhanVienUpdateService _updateService;
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IResetMatKhauService _resetService;
        public NhanVienController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, INhanVienTaiKhoanService service,
            INhanVienUpdateService updateService, ITaiKhoanService taiKhoanService, IResetMatKhauService resetService)
        {
            _service = service;
            _context = context;
            _userManager = userManager;
            _updateService = updateService;
            _taiKhoanService = taiKhoanService;
            _resetService = resetService;
        }

        // GET: /NhanVien
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách nhân viên
            var nhanViens = await _context.NhanViens
                .OrderBy(x => x.hoTen)
                .ToListAsync();

            // Lấy user gắn với nhân viên
            var users = await _userManager.Users
                .Where(x => x.nhanVienId != null)
                .ToListAsync();

            var result = nhanViens.Select(nv =>
            {
                var user = users.FirstOrDefault(u => u.nhanVienId == nv.nhanVienId);

                var biKhoa = false;
                if (user != null)
                {
                    biKhoa = user.LockoutEnd != null &&
                             user.LockoutEnd > DateTimeOffset.Now;
                }

                return new NhanVienListItemVm
                {
                    NhanVienId = nv.nhanVienId,
                    HoTen = nv.hoTen,
                    VaiTro = nv.vaiTro,
                    LuongCoBan = nv.luongCoBan,
                    HoatDong = nv.hoatDong,
                    CoTaiKhoan = user != null,
                    EmailDangNhap = user?.Email,
                    // ⭐ quyết định nút khóa/mở
                    BiKhoaTaiKhoan = biKhoa
                };
            }).ToList();

            return View(result);
        }

        // GET: /NhanVien/Tao
        public IActionResult Tao()
        {
            var vm = new TaoNhanVienTaiKhoanVm
            {
                DanhSachVaiTro = new[] { "BacSi", "KyThuatVien", "LeTan" },
                DanhSachRole = new[] { "Admin", "BacSi", "KyThuatVien", "LeTan" }
            };

            return View(vm);
        }

        // POST: /NhanVien/Tao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tao(TaoNhanVienTaiKhoanVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.DanhSachVaiTro = new[] { "BacSi", "KyThuatVien", "LeTan" };
                vm.DanhSachRole = new[] { "Admin", "BacSi", "KyThuatVien", "LeTan" };
                return View(vm);
            }

            try
            {
                await _service.TaoNhanVienVaTaiKhoanAsync(new TaoNhanVienTaiKhoanDto
                {
                    hoTen = vm.HoTen,
                    vaiTro = vm.VaiTroNhanVien,
                    luongCoBan = vm.LuongCoBan,
                    email = vm.Email,
                    matKhau = vm.MatKhau,
                    role = vm.RoleDangNhap
                });

                TempData["Success"] = "Tao nhan vien va tai khoan thanh cong";
                return RedirectToAction("Tao");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.DanhSachVaiTro = new[] { "BacSi", "KyThuatVien", "LeTan" };
                vm.DanhSachRole = new[] { "Admin", "BacSi", "KyThuatVien", "LeTan" };
                return View(vm);
            }
        }

        // GET: /NhanVien/Sua/5
        public async Task<IActionResult> Sua(int id)
        {
            var nv = await _context.NhanViens.FirstOrDefaultAsync(x => x.nhanVienId == id);
            if (nv == null) return NotFound();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.nhanVienId == id);
            string roleName = string.Empty;
            if (user != null) 
            { 
                var roles = await _userManager.GetRolesAsync(user);
                roleName = roles.FirstOrDefault() ?? "";
            }

            var vm = new SuaNhanVienVm
            {
                NhanVienId = nv.nhanVienId,
                HoTen = nv.hoTen,
                VaiTroNhanVien = nv.vaiTro,
                LuongCoBan = nv.luongCoBan,
                HoatDong = nv.hoatDong,
                RoleDangNhap = roleName,

                DanhSachVaiTroNhanVien = new[] { "BacSi", "KyThuatVien", "LeTan" },
                DanhSachRoleDangNhap = new[] { "Admin", "BacSi", "KyThuatVien", "LeTan" }
            };

            return View(vm);
        }

        // POST: /NhanVien/Sua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(SuaNhanVienVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.DanhSachVaiTroNhanVien = new[] { "BacSi", "KyThuatVien", "LeTan" };
                vm.DanhSachRoleDangNhap = new[] { "Admin", "BacSi", "KyThuatVien", "LeTan" };
                return View(vm);
            }

            try
            {
                await _updateService.CapNhatNhanVienVaRoleAsync(
                    vm.NhanVienId,
                    vm.HoTen,
                    vm.VaiTroNhanVien,
                    vm.LuongCoBan,
                    vm.HoatDong,
                    vm.RoleDangNhap
                );

                TempData["Success"] = "Cap nhat nhan vien thanh cong";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.DanhSachVaiTroNhanVien = new[] { "BacSi", "KyThuatVien", "LeTan" };
                vm.DanhSachRoleDangNhap = new[] { "Admin", "BacSi", "KyThuatVien", "LeTan" };
                return View(vm);
            }
        }
        /// <summary>
        /// // POST: /NhanVien/KhoaTaiKhoan/5
        /// NhanVien.HoatDong	Nhân viên còn làm việc không
        //Identity.LockoutEnd Có login được không
        /// </summary>
        /// <param name="nhanVienId"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhoaTaiKhoan(int nhanVienId)
        {
            try
            {
                var adminUserId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _taiKhoanService.KhoaTaiKhoanAsync(
                    nhanVienId,
                    adminUserId);

                TempData["Success"] = "Da khoa tai khoan";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: /NhanVien/MoKhoaTaiKhoan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoKhoaTaiKhoan(int nhanVienId)
        {
            try
            {
                await _taiKhoanService.MoKhoaTaiKhoanAsync(nhanVienId);
                TempData["Success"] = "Da mo khoa tai khoan";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
        // GET: /NhanVien/ResetMatKhau/5
        public async Task<IActionResult> ResetMatKhau(int id)
        {
            var nv = await _context.NhanViens
                .FirstOrDefaultAsync(x => x.nhanVienId == id);

            if (nv == null)
                return NotFound();

            var vm = new ResetMatKhauVm
            {
                NhanVienId = nv.nhanVienId,
                Email = (await _context.Users
                    .FirstOrDefaultAsync(u => u.nhanVienId == id))?.Email
            };

            return View(vm);
        }

        // POST: /NhanVien/ResetMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetMatKhau(ResetMatKhauVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _resetService.ResetAsync(
                    vm.NhanVienId,
                    vm.MatKhauMoi);

                TempData["Success"] = "Da reset mat khau thanh cong";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
    }
}