using ClinicManager.Data;
using ClinicManager.Services.Luong;
using ClinicManager.ViewModels.Luong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BangLuongController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILuongService _luongService;

        public BangLuongController(ApplicationDbContext context, ILuongService luongService)
        {
            _context = context;
            _luongService = luongService;
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
                    TenNhanVien = g.First().NhanVien.hoTen,
                    TongLuong = g.Sum(x => x.soTien),
                    ChiTiets = g.ToList()
                })
                .OrderByDescending(x => x.TongLuong)
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
            await _luongService.TinhLuongThangAsync(thang, nam);
            TempData["ToastSuccess"] = "Da tinh luong thang " + thang + "/" + nam;
            return RedirectToAction(nameof(Index));
        }

        // ==================================================
        // 4. CHỐT LƯƠNG
        // ==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChotLuong(int id)
        {
            await _luongService.ChotLuongAsync(id);
            TempData["ToastSuccess"] = "Da chot bang luong";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        // ==================================================
        // 5. MỞ CHỐT (ADMIN)
        // ==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoChotLuong(int id)
        {
            await _luongService.MoChotLuongAsync(id);
            TempData["ToastWarning"] = "Da mo chot bang luong";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }
    }
}
