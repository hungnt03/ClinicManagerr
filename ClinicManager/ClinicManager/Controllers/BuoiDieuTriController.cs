using System.Security.Claims;
using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Models.Entities;
using ClinicManager.Services;
using ClinicManager.ViewModels.BuoiDieuTri;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Controllers
{
    [Authorize]
    public class BuoiDieuTriController : Controller
    {
        private readonly IBuoiDieuTriService _buoiDieuTriService;
        private readonly IDotDieuTriService _dotDieuTriService;
        private readonly INhanVienService _nhanVienService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BuoiDieuTriController(
            IBuoiDieuTriService buoiDieuTriService,
            IDotDieuTriService dotDieuTriService,
            INhanVienService nhanVienService, 
            ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _buoiDieuTriService = buoiDieuTriService;
            _dotDieuTriService = dotDieuTriService;
            _nhanVienService = nhanVienService;
            _context = context;
            _userManager = userManager;
        }

        // ==================================================
        // 1. TẠO BUỔI ĐIỀU TRỊ
        // ==================================================
        [HttpGet]
        public async Task<IActionResult> Create(int dotDieuTriId, int benhNhanId)
        {
            var vm = new BuoiDieuTriCreateVm
            {
                DotDieuTriId = dotDieuTriId,
                BenhNhanId = benhNhanId,
                NgayDieuTri = DateTime.Today,
                DanhSachBacSi = await _nhanVienService.GetDanhSachBacSiKyThuatViensync(),
                DanhSachKyThuatVien = await _nhanVienService.GetDanhSachBacSiKyThuatViensync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuoiDieuTriCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.DanhSachBacSi = await _nhanVienService.GetDanhSachBacSiKyThuatViensync();
                vm.DanhSachKyThuatVien = await _nhanVienService.GetDanhSachBacSiKyThuatViensync();
                return View(vm);
            }
                

            var buoiId = await _buoiDieuTriService.TaoBuoiDieuTriAsync(
                vm.DotDieuTriId,
                vm.BenhNhanId,
                vm.NgayDieuTri,
                vm.BacSiDieuTriTayId,
                vm.KyThuatVienTapId,
                vm.NoiDungTap,
                vm.NoiDungDieuTriTay,
                vm.ChiDinhDacBiet
            );

            // ⭐ quay lại Create nhưng đã có BuoiDieuTriId để thêm vật tư
            vm.BuoiDieuTriId = buoiId;
            vm.DanhSachBacSi = await _nhanVienService.GetDanhSachBacSiKyThuatViensync();
            vm.DanhSachKyThuatVien = await _nhanVienService.GetDanhSachBacSiKyThuatViensync();

            ViewBag.DaTaoBuoi = true;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ThemVatTu([FromBody] ThemVatTuVm vm)
        {
            if (vm.BuoiDieuTriId <= 0 || vm.VatTuId <= 0 || vm.SoLuong <= 0)
                return BadRequest("Du lieu khong hop le");

            try
            {
                await _buoiDieuTriService.ThemVatTuAsync(
                    vm.BuoiDieuTriId,
                    vm.VatTuId,
                    vm.SoLuong
                );

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> DanhSachVatTu(int buoiDieuTriId)
        {
            var ds = await _context.ThuocVatTuBuoiDieuTris
                .Where(x => x.buoiDieuTriId == buoiDieuTriId)
                .Select(x => new
                {
                    x.id,
                    x.VatTu.tenVatTu,
                    x.soLuong,
                    x.donGia
                })
                .ToListAsync();

            return PartialView("_DanhSachVatTu", ds);
        }

        // ==================================================
        // GET: SUA BUOI DIEU TRI (ADMIN)
        // ==================================================
        [Authorize(Roles = "Admin,BacSi")]
        [HttpGet]
        public async Task<IActionResult> Edit(int buoiDieuTriId)
        {
            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == buoiDieuTriId);

            if (buoi == null)
                return NotFound();

            var vm = new BuoiDieuTriEditVm
            {
                BuoiDieuTriId = buoi.buoiDieuTriId,
                DotDieuTriId = buoi.dotDieuTriId,
                BenhNhanId = buoi.benhNhanId,

                NgayDieuTri = buoi.ngayDieuTri,

                BacSiDieuTriTayId = buoi.bacSiDieuTriTayId,
                KyThuatVienTapId = buoi.kyThuatVienTapId,

                NoiDungTap = buoi.noiDungTap,
                NoiDungDieuTriTay = buoi.noiDungDieuTriTay,
                ChiDinhDacBiet = buoi.chiDinhDacBiet,

                ChiPhiThuocVatTu = buoi.chiPhiThuocVatTu
            };

            // load dropdown
            vm.DanhSachBacSiDieuTriTay =
                await _nhanVienService.GetDanhSachBacSiKyThuatViensync();

            vm.DanhSachNguoiTap =
                await _nhanVienService.GetDanhSachBacSiKyThuatViensync();

            return View(vm);
        }


        [Authorize(Roles = "Admin,BacSi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BuoiDieuTriEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.DanhSachBacSiDieuTriTay =
                    await _nhanVienService.GetDanhSachBacSiKyThuatViensync();

                vm.DanhSachNguoiTap =
                    await _nhanVienService.GetDanhSachBacSiKyThuatViensync();

                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var adminUser = await _userManager.FindByIdAsync(userId);
            var adminNhanVienId = adminUser.nhanVienId.Value;

            await _buoiDieuTriService.CapNhatBuoiDieuTriAsync(
                vm,
                adminNhanVienId
            );

            return RedirectToAction("ChiTiet", "DotDieuTri",
                new { id = vm.DotDieuTriId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Audit(int buoiDieuTriId)
        {
            var audits = await _context.BuoiDieuTriAudits
                .Where(x => x.buoiDieuTriId == buoiDieuTriId)
                .OrderByDescending(x => x.suaLuc)
                .ToListAsync();

            return View(audits);
        }


    }
}
