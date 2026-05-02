using ClinicManager.Data;
using ClinicManager.Services;
using ClinicManager.ViewModels.DotDieuTri;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Controllers
{
    [Authorize]
    public class DotDieuTriController : Controller
    {
        private readonly IDotDieuTriService _dotDieuTriService;
        private readonly IBenhNhanService _benhNhanService;
        private readonly INhanVienService _nhanVienService;
        private readonly IGoiDieuTriService _goiDieuTriService;
        private readonly ApplicationDbContext _context;

        public DotDieuTriController(
            IDotDieuTriService dotDieuTriService,
            IBenhNhanService benhNhanService,
            INhanVienService nhanVienService,
            IGoiDieuTriService goiDieuTriService, 
            ApplicationDbContext context)
        {
            _dotDieuTriService = dotDieuTriService;
            _benhNhanService = benhNhanService;
            _nhanVienService = nhanVienService;
            _goiDieuTriService = goiDieuTriService;
            _context = context;
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

            try
            {
                var dotId = await _dotDieuTriService.TaoDotDieuTriAsync(
                    vm.BenhNhanId,
                    vm.BacSiKhamId,
                    vm.NgayKham,
                    vm.TienSuBenh,
                    vm.ChanDoan,
                    vm.PhacDoDieuTri,
                    vm.GoiDieuTriId,
                    vm.PhanTramGiamGia
                );

                // 👉 sau khi khám xong → sang chi tiết đợt
                return RedirectToAction("ChiTiet", new { id = dotId });
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;

                vm.BacSiList = await _nhanVienService.GetDanhSachBacSiKyThuatViensync();
                vm.GoiDieuTriList = await _goiDieuTriService.GetGoiHoatDongAsync();
                return View(vm);
            }
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var dot = await _context.DotDieuTris
                .Where(x => x.dotDieuTriId == id)
                .Select(x => new DotDieuTriChiTietVm
                {
                    DotDieuTriId = x.dotDieuTriId,
                    BenhNhanId = x.benhNhanId,

                    NgayKham = x.ngayKham,
                    TienSuBenh = x.tienSuBenh,
                    ChanDoan = x.chanDoan,
                    PhacDoDieuTri = x.phacDoDieuTri,

                    TongSoBuoi = x.tongSoBuoi,
                    SoBuoiDaDung = x.soBuoiDaDung,

                    TrangThai = x.trangThai.ToString(),

                    BuoiDieuTris = x.BuoiDieuTris
                        .OrderByDescending(b => b.ngayDieuTri) // ⭐ gần nhất trước
                        .Select(b => new BuoiDieuTriItemVm
                        {
                            BuoiDieuTriId = b.buoiDieuTriId,
                            NgayDieuTri = b.ngayDieuTri,

                            NoiDungTap = b.noiDungTap,
                            NoiDungDieuTriTay = b.noiDungDieuTriTay,
                            ChiDinhDacBiet = b.chiDinhDacBiet,

                            TenBacSiDieuTriTay = b.BacSiDieuTriTay != null
                                ? b.BacSiDieuTriTay.hoTen
                                : null,

                            TenNguoiTap = b.NguoiTap != null
                                ? b.NguoiTap.hoTen
                                : null,

                            ChiPhiThuocVatTu = b.chiPhiThuocVatTu
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (dot == null) return NotFound();

            return View(dot);
        }

    }
}