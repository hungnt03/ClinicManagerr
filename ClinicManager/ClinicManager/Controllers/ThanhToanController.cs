using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.Services;
using ClinicManager.ViewModels.ThanhToan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Controllers
{
    [Authorize]
    public class ThanhToanController : Controller
    {
        private readonly IThanhToanService _thanhToanService;
        private readonly IDotDieuTriService _dotDieuTriService;
        private readonly ApplicationDbContext _context;

        public ThanhToanController(
            IThanhToanService thanhToanService,
            IDotDieuTriService dotDieuTriService,
            ApplicationDbContext context)
        {
            _thanhToanService = thanhToanService;
            _dotDieuTriService = dotDieuTriService;
            _context = context;
        }

        // =============================
        // GET: MODAL THU TIỀN GÓI
        // =============================
        [HttpGet]
        public async Task<IActionResult> ThuTienGoi(int dotDieuTriId)
        {
            var dot = await _dotDieuTriService.GetByIdAsync(dotDieuTriId);
            if (dot == null) return NotFound();

            var vm = new ThuTienGoiVm
            {
                DotDieuTriId = dot.dotDieuTriId,
                TongTien = dot.tongTien,
                PhanTramGiamGia = dot.phanTramGiamGia,
                DaThanhToan = dot.daThanhToan,
                SoTienThu =
                    (dot.tongTien
                        - (dot.tongTien * dot.phanTramGiamGia / 100))
                        - dot.daThanhToan
            };


            return PartialView("_ThuTienGoiModal", vm);
        }

        // =============================
        // POST: THU TIỀN GÓI
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThuTienGoi(ThuTienGoiVm vm)
        {
            var dot = await _dotDieuTriService.GetByIdAsync(vm.DotDieuTriId);
            if (dot == null) return BadRequest("Dot dieu tri khong ton tai");
            var tongSauGiam =
                dot.tongTien - (dot.tongTien * dot.phanTramGiamGia / 100);
            var conLai = tongSauGiam - dot.daThanhToan;
            if (vm.SoTienThu > conLai)
                return BadRequest("So tien thu vuot qua so tien con lai");

            if (!ModelState.IsValid)
                return BadRequest("Du lieu khong hop le");

            var thanhToanId = await _thanhToanService.ThuTienGoiAsync(
                vm.DotDieuTriId,
                vm.SoTienThu,
                vm.HinhThuc,
                vm.GhiChu
            );

            return Ok(new { thanhToanId });
        }

        // =============================
        // GET: MODAL THU TIỀN THUỐC
        // =============================
        [HttpGet]
        public async Task<IActionResult> ThuTienThuoc(int buoiDieuTriId)
        {
            var buoi = await _context.BuoiDieuTris
                .FirstOrDefaultAsync(x => x.buoiDieuTriId == buoiDieuTriId);

            if (buoi == null) return NotFound();

            var daThu = await _context.ThanhToans
                .Where(x =>
                    x.loai == LoaiThanhToan.ThuocVatTu &&
                    x.buoiDieuTriId == buoiDieuTriId)
                .SumAsync(x => (decimal?)x.soTien) ?? 0;

            var vm = new ThuTienThuocVm
            {
                BuoiDieuTriId = buoi.buoiDieuTriId,
                TongChiPhi = buoi.chiPhiThuocVatTu,
                DaThu = daThu,
                SoTienThu = buoi.chiPhiThuocVatTu - daThu
            };

            return PartialView("_ThuTienThuocModal", vm);
        }

        // =============================
        // POST: THU TIỀN THUỐC
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThuTienThuoc(ThuTienThuocVm vm)
        {
            if (!ModelState.IsValid)
                return BadRequest("Du lieu khong hop le");

            var thanhToanId = await _thanhToanService.ThuTienThuocVatTuAsync(
                vm.BuoiDieuTriId,
                vm.SoTienThu,
                vm.HinhThuc,
                vm.GhiChu
            );

            return Ok(new { thanhToanId });
        }
        [HttpGet]
        public async Task<IActionResult> PhieuThu(int id)
        {
            var tt = await _context.ThanhToans
                .FirstOrDefaultAsync(x => x.thanhToanId == id);

            if (tt == null) return NotFound();

            string tenBenhNhan = "";
            string noiDung = "";

            if (tt.loai == LoaiThanhToan.GoiDieuTri)
            {
                var dot = await _context.DotDieuTris
                    .Include(x => x.BenhNhan)
                    .FirstAsync(x => x.dotDieuTriId == tt.dotDieuTriId);

                tenBenhNhan = dot.BenhNhan.hoTen;
                noiDung = $"Thu tien goi dieu tri (Dot #{dot.dotDieuTriId})";
            }
            else
            {
                var buoi = await _context.BuoiDieuTris
                    .Include(x => x.BenhNhan)
                    .FirstAsync(x => x.buoiDieuTriId == tt.buoiDieuTriId);

                tenBenhNhan = buoi.BenhNhan.hoTen;
                noiDung = $"Thu tien thuoc/vat tu (Buoi #{buoi.buoiDieuTriId})";
            }

            var vm = new PhieuThuVm
            {
                ThanhToanId = tt.thanhToanId,
                SoPhieu = $"PT-{tt.thanhToanId:000000}",
                NgayThu = tt.ngayThu,
                TenBenhNhan = tenBenhNhan,
                NoiDungThu = noiDung,
                SoTien = tt.soTien,
                HinhThuc = tt.hinhThuc,
                GhiChu = tt.ghiChu
            };

            return View(vm);
        }
    }
}
