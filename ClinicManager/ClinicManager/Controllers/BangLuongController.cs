using ClinicManager.Data;
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

        public BangLuongController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
