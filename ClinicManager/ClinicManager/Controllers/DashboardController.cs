using System.Security.Claims;
using ClinicManager.Data;
using ClinicManager.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Today;
            int thang = now.Month;
            int nam = now.Year;

            // ===== KPI =====
            var doanhThuThang = await _context.DotDieuTris
                .Where(x =>
                    x.ngayThanhToan.HasValue &&
                    x.ngayThanhToan.Value.Month == thang &&
                    x.ngayThanhToan.Value.Year == nam)
                .SumAsync(x => (decimal?)x.daThanhToan) ?? 0;

            var chiPhiLuongThang = await _context.BangLuongThangs
                .Where(x => x.thang == thang && x.nam == nam)
                .SumAsync(x => (decimal?)x.tongLuong) ?? 0;

            var soBenhNhan = await _context.DotDieuTris
                .Where(x => x.ngayKham.Month == thang && x.ngayKham.Year == nam)
                .Select(x => x.benhNhanId)
                .Distinct()
                .CountAsync();

            var soBuoi = await _context.BuoiDieuTris
                .Where(x => x.ngayDieuTri.Month == thang && x.ngayDieuTri.Year == nam)
                .CountAsync();

            // ===== CHART THEO 12 THÁNG =====
            var labels = Enumerable.Range(1, 12)
                .Select(m => $"Th {m}")
                .ToList();

            var doanhThuSeries = new List<decimal>();
            var chiPhiLuongSeries = new List<decimal>();
            var benhNhanSeries = new List<int>();

            foreach (var m in Enumerable.Range(1, 12))
            {
                doanhThuSeries.Add(
                    await _context.DotDieuTris
                        .Where(x =>
                            x.ngayThanhToan.HasValue &&
                            x.ngayThanhToan.Value.Month == m &&
                            x.ngayThanhToan.Value.Year == nam)
                        .SumAsync(x => (decimal?)x.daThanhToan) ?? 0
                );

                chiPhiLuongSeries.Add(
                    await _context.BangLuongThangs
                        .Where(x => x.thang == m && x.nam == nam)
                        .SumAsync(x => (decimal?)x.tongLuong) ?? 0
                );

                benhNhanSeries.Add(
                    await _context.DotDieuTris
                        .Where(x => x.ngayKham.Month == m && x.ngayKham.Year == nam)
                        .Select(x => x.benhNhanId)
                        .Distinct()
                        .CountAsync()
                );
            }


            var vm = new AdminDashboardVm
            {
                DoanhThuThang = doanhThuThang,
                ChiPhiLuongThang = chiPhiLuongThang,
                SoBenhNhanThang = soBenhNhan,
                SoBuoiDieuTriThang = soBuoi,

                Labels = labels,
                DoanhThuSeries = doanhThuSeries,
                ChiPhiLuongSeries = chiPhiLuongSeries,
                BenhNhanSeries = benhNhanSeries
            };

            return View(vm);
        }

        [Authorize(Roles = "BacSi,KyThuatVien")]
        public async Task<IActionResult> BacSi()
        {
            // 1️⃣ Lấy userId của Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2️⃣ Lấy nhanVienId từ AspNetUsers
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.nhanVienId })
                .FirstOrDefaultAsync();

            if (user?.nhanVienId == null)
                return Unauthorized("Tai khoan chua lien ket nhan vien");

            int nhanVienId = user.nhanVienId.Value;

            var today = DateTime.Today;

            // 3️⃣ Lấy các buổi bác sĩ tham gia
            var buoiHomNay = await _context.BuoiDieuTris
                .Include(x => x.DotDieuTri)
                    .ThenInclude(d => d.BenhNhan)
                .Where(x =>
                    x.ngayDieuTri.Date == today &&
                    (x.bacSiDieuTriTayId == nhanVienId ||
                     x.DotDieuTri.bacSiKhamId == nhanVienId))
                .OrderBy(x => x.ngayDieuTri)
                .Select(x => new BuoiDieuTriHomNayVm
                {
                    BuoiDieuTriId = x.buoiDieuTriId,
                    DotDieuTriId = x.dotDieuTriId,
                    TenBenhNhan = x.DotDieuTri.BenhNhan.hoTen,
                    NgayDieuTri = x.ngayDieuTri,
                    DieuTriTay = x.bacSiDieuTriTayId.HasValue,
                    Tap = x.kyThuatVienTapId.HasValue,
                    TrangThai = "Chua hoan thanh"
                })
                .ToListAsync();

            var vm = new DashboardBacSiVm
            {
                SoBuoiDieuTriHomNay = buoiHomNay.Count,
                SoBenhNhanHomNay = buoiHomNay
                    .Select(x => x.TenBenhNhan)
                    .Distinct()
                    .Count(),
                BuoiHomNay = buoiHomNay
            };

            return View(vm);
        }

        [Authorize(Roles = "BacSi,KyThuatVien")]
        [HttpGet]
        public async Task<IActionResult> BacSiCalendarEvents(DateTime start, DateTime end)
        {
            // 1️⃣ Lấy nhanVienId từ Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var nhanVienId = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.nhanVienId)
                .FirstOrDefaultAsync();

            if (nhanVienId == null)
                return Unauthorized();

            // 2️⃣ Lấy buổi điều trị liên quan bác sĩ
            var events = await _context.BuoiDieuTris
                .Include(x => x.DotDieuTri)
                    .ThenInclude(d => d.BenhNhan)
                .Where(x =>
                    x.ngayDieuTri >= start &&
                    x.ngayDieuTri <= end &&
                    (
                        x.bacSiDieuTriTayId == nhanVienId ||
                        x.DotDieuTri.bacSiKhamId == nhanVienId
                    ))
                .Select(x => new BacSiCalendarEventVm
                {
                    id = x.buoiDieuTriId,
                    start = x.ngayDieuTri,
                    title = x.DotDieuTri.BenhNhan.hoTen,
                    color =
                        x.bacSiDieuTriTayId == nhanVienId &&
                        x.DotDieuTri.bacSiKhamId == nhanVienId
                            ? "#ffc107"   // vừa khám + điều trị
                            : x.bacSiDieuTriTayId == nhanVienId
                                ? "#28a745" // điều trị tay
                                : "#0d6efd",// khám
                    url = $"/BuoiDieuTri/Edit?buoiDieuTriId={x.buoiDieuTriId}"
                })
                .ToListAsync();

            return Json(events);
        }
    }
}
