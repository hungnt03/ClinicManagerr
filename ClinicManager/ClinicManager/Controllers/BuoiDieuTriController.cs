using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.Services;
using ClinicManager.ViewModels.BuoiDieuTri;
using Microsoft.AspNetCore.Authorization;
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

        public BuoiDieuTriController(
            IBuoiDieuTriService buoiDieuTriService,
            IDotDieuTriService dotDieuTriService,
            INhanVienService nhanVienService, 
            ApplicationDbContext context)
        {
            _buoiDieuTriService = buoiDieuTriService;
            _dotDieuTriService = dotDieuTriService;
            _nhanVienService = nhanVienService;
            _context = context;
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

            // sau khi tạo buổi → về chi tiết đợt
            return RedirectToAction("ChiTiet", "DotDieuTri", new { id = vm.DotDieuTriId });
        }

        [HttpPost]
        public async Task<IActionResult> ThemVatTu([FromBody] ThemVatTuVm vm)
        {
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

    }
}
