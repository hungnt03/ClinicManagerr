using ClinicManager.Services;
using ClinicManager.ViewModels.DotDieuTri;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers
{
    [Authorize]
    public class DotDieuTriController : Controller
    {
        private readonly IDotDieuTriService _dotDieuTriService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly INhanVienService _nhanVienService;
        private readonly IGoiDieuTriService _goiDieuTriService;

        public DotDieuTriController(
            IDotDieuTriService dotDieuTriService,
            IBenhNhanService benhNhanService,
            INhanVienService nhanVienService,
            IGoiDieuTriService goiDieuTriService)
        {
            _dotDieuTriService = dotDieuTriService;
            _benhNhanService = benhNhanService;
            _nhanVienService = nhanVienService;
            _goiDieuTriService = goiDieuTriService;
        }

        // ==================================================
        // 1. KHÁM – TẠO ĐỢT ĐIỀU TRỊ
        // ==================================================
        [HttpGet]
        public async Task<IActionResult> Create(int benhNhanId)
        {
            var benhNhan = await _benhNhanService.GetByIdAsync(benhNhanId);
            if (benhNhan == null) return NotFound();

            var vm = new DotDieuTriCreateVm
            {
                BenhNhanId = benhNhanId,
                TenBenhNhan = benhNhan.hoTen,
                BacSiList = await _nhanVienService.GetDanhSachBacSiKyThuatViensync(),
                GoiDieuTriList = await _goiDieuTriService.GetGoiHoatDongAsync(),
                NgayKham = DateTime.Today
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DotDieuTriCreateVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.BacSiList = await _nhanVienService.GetDanhSachBacSiKyThuatViensync();
                vm.GoiDieuTriList = await _goiDieuTriService.GetGoiHoatDongAsync();
                return View(vm);
            }

            var dotId = await _dotDieuTriService.TaoDotDieuTriAsync(
                vm.BenhNhanId,
                vm.BacSiKhamId,
                vm.NgayKham,
                vm.ChanDoan,
                vm.PhacDoDieuTri,
                vm.GoiDieuTriId,
                vm.DaThanhToan
            );

            // 👉 sau khi khám xong → sang chi tiết đợt
            return RedirectToAction("ChiTiet", new { id = dotId });
        }

        // ==================================================
        // 2. CHI TIẾT ĐỢT ĐIỀU TRỊ
        // ==================================================
        public async Task<IActionResult> ChiTiet(int id)
        {
            var dot = await _benhNhanService.GetChiTietDotDieuTriAsync(id);
            if (dot == null) return NotFound();

            return View(dot);
        }
    }
}