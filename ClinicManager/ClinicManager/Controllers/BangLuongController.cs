using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.Services.Luong;
using ClinicManager.Utils;
using ClinicManager.ViewModels.Luong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ClinicManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BangLuongController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILuongService _luongService;
        private readonly IWebHostEnvironment _env;

        public BangLuongController(ApplicationDbContext context, ILuongService luongService, IWebHostEnvironment env)
        {
            _context = context;
            _luongService = luongService;
            _env = env;
        }

        // ==================================================
        // 1. DANH SÁCH BẢNG LƯƠNG
        // ==================================================
        public async Task<IActionResult> Index()
        {
            var list = await _context.BangLuongThangs
                .OrderByDescending(x => x.nam)
                .ThenByDescending(x => x.thang)
                .Select(x => new BangLuongThangListItemVm
                {
                    BangLuongThangId = x.bangLuongThangId,
                    Thang = x.thang,
                    Nam = x.nam,
                    DaChot = x.daChot,
                    TaoLuc = x.taoLuc,

                    SoNhanVien = x.ChiTiets
                        .Select(c => c.nhanVienId)
                        .Distinct()
                        .Count(),

                    TongLuong = x.ChiTiets.Sum(c => c.soTien)
                })
                .ToListAsync();

            return View(list);
        }

        // ==================================================
        // 2. CHI TIẾT BẢNG LƯƠNG THÁNG
        // ==================================================
        public async Task<IActionResult> ChiTiet(int id)
        {
            var bangLuong = await _context.BangLuongThangs
                .Include(x => x.ChiTiets)
                    .ThenInclude(c => c.NhanVien)
                .FirstOrDefaultAsync(x => x.bangLuongThangId == id);

            if (bangLuong == null)
                return NotFound();

            var vm = new BangLuongThangChiTietVm
            {
                BangLuongThangId = bangLuong.bangLuongThangId,
                Thang = bangLuong.thang,
                Nam = bangLuong.nam,
                DaChot = bangLuong.daChot
            };

            vm.NhanViens = bangLuong.ChiTiets
                .GroupBy(x => x.nhanVienId)
                .Select(g => new BangLuongNhanVienVm
                {
                    nhanVienId = g.First().nhanVienId,
                    TenNhanVien = g.First().NhanVien.hoTen,
                    TongLuong = g.Sum(x => x.soTien),
                    ChiTiets = g.OrderBy(x => x.loai).ToList()
                })
                .OrderBy(x => x.nhanVienId)
                .ToList();

            return View(vm);
        }

        // ==================================================
        // 3. TÍNH LƯƠNG THÁNG
        // ==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TinhLuong(int thang, int nam)
        {
            try
            {
                await _luongService.TinhLuongThangAsync(thang, nam);
                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "Đã tính lương tháng " + thang + "/" + nam;
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemPhatSinh(ThemPhatSinhVm vm)
        {
            try
            {
                if (vm.SoTien <= 0) throw new Exception("Số tiền phải lớn hơn 0");

                // 1. Kiểm tra bảng lương đã chốt chưa
                var bangLuong = await _context.BangLuongThangs
                    .FirstOrDefaultAsync(x => x.bangLuongThangId == vm.BangLuongThangId);

                if (bangLuong == null || bangLuong.daChot)
                    throw new Exception("Bảng lương không tồn tại hoặc đã chốt, không thể thêm phát sinh.");

                // 2. Tạo dòng chi tiết lương mới
                var chiTiet = new BangLuongThangChiTiet
                {
                    bangLuongThangId = vm.BangLuongThangId,
                    nhanVienId = vm.NhanVienId,
                    loai = LoaiLuongChiTiet.PhatSinh,
                    soTien = vm.Loai == 2 ? -vm.SoTien : vm.SoTien, // Loại 2 là Trừ -> lưu số âm
                    dienGiai = vm.NoiDung
                };

                _context.BangLuongThangChiTiets.Add(chiTiet);
                await _context.SaveChangesAsync();

                // 3. Cập nhật lại tổng lương của bảng chính
                await _luongService.UpdateTongLuongNhanVienAsync(vm.BangLuongThangId);

                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "Đã thêm khoản phát sinh thành công";
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(ChiTiet), new { id = vm.BangLuongThangId });
        }


        // ==================================================
        // 4. CHỐT LƯƠNG
        // ==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChotLuong(int id)
        {
            try
            {
                await _luongService.ChotLuongAsync(id);
                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "Đã chốt bảng lương";
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        // ==================================================
        // 5. MỞ CHỐT (ADMIN)
        // ==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoChotLuong(int id)
        {
            try
            {
                await _luongService.MoChotLuongAsync(id);
                TempData["ToastType"] = "warning";
                TempData["ToastMessage"] = "Đã mở chốt bảng lương";
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
            }
            return View();
        }

        public async Task<IActionResult> XuatExcel(int id)
        {
            var result = new XuatExcelVM();
            var bangLuong = await _context.BangLuongThangs.FirstOrDefaultAsync(x => x.bangLuongThangId == id);
            if (bangLuong == null) throw new Exception("Không tìm thấy bảng lương");
            var range = bangLuong.thang.GetMonthRange(bangLuong.nam);
            var stDate = range.StartDate;
            var edDate = range.EndDate;
            var cauHinh = _context.CauHinhLuongs
                .Where(x => x.apDungTuNgay <= stDate)
                .OrderByDescending(x => x.apDungTuNgay)
                .FirstOrDefault();

            try
            {
                result.BangLuongThangId = id;
                result.NgayThang = new DateTime(bangLuong.nam, bangLuong.thang, 1);
                result.cfg = cauHinh;
                result.ThuList = await _luongService.TaoThuExcelAsync(id, stDate, edDate);
                result.ChiList = await _luongService.TaoChiExcelAsync(id, stDate, edDate);
                result.ChamCongList = await _luongService.TaoChamCongExcelAsync(id, stDate, edDate);
                result.TheoDoiDieuTriList = await _luongService.TaoTheoDoiDieuTriExcelVMAsync(id, stDate, edDate);
                result.LuongList = await _luongService.TaoLuongExcelVMAsync(id, stDate, edDate);

                var templatePath = Path.Combine(_env.WebRootPath, "Templates", "Luong.xlsx");
                byte[] fileBytes = await _luongService.TaoFileLuongExcelAsync(result, templatePath);
                string fileName = $"BangLuong_{bangLuong.thang}_{bangLuong.nam}.xlsx";

                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "Tạo file lương thành công";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
                return RedirectToAction(nameof(ChiTiet), new { id = id });
            }
        }
    }

}
