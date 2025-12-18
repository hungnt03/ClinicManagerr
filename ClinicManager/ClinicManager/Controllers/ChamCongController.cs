using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Models.Entities;
using ClinicManager.Services;
using ClinicManager.ViewModels.ChamCong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicManager.Controllers
{
    [Authorize]
    public class ChamCongController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChamCongController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===============================
        // helper
        // ===============================
        private async Task<int> GetNhanVienIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            return user!.nhanVienId!.Value;
        }

        // ===============================
        // 1. CHẤM CÔNG HÔM NAY
        // ===============================
        public async Task<IActionResult> Index()
        {
            var nhanVienId = await GetNhanVienIdAsync();
            var today = DateTime.Today;

            var chamCong = await _context.ChamCongs
                .FirstOrDefaultAsync(x =>
                    x.nhanVienId == nhanVienId &&
                    x.thoiGianVao.Date == today);

            var vm = new ChamCongHomNayVm();

            if (chamCong != null)
            {
                vm.DaCheckIn = !chamCong.nghiPhep;
                vm.DaCheckOut = chamCong.thoiGianRa.HasValue;
                vm.GioVao = chamCong.thoiGianVao;
                vm.GioRa = chamCong.thoiGianRa;
                vm.NghiPhep = chamCong.nghiPhep;
                vm.AnTrua = chamCong.anTrua;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(bool anTrua)
        {
            var nhanVienId = await GetNhanVienIdAsync();
            var today = DateTime.Today;

            var tonTai = await _context.ChamCongs.AnyAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == today);

            if (tonTai)
            {
                TempData["Error"] = "Da cham cong hom nay";
                return RedirectToAction("Index");
            }

            _context.ChamCongs.Add(new ChamCong
            {
                nhanVienId = nhanVienId,
                thoiGianVao = DateTime.Now,
                anTrua = anTrua,
                nghiPhep = false,
                nghiPhepCoLuong = false
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var nhanVienId = await GetNhanVienIdAsync();
            var today = DateTime.Today;

            var chamCong = await _context.ChamCongs.FirstOrDefaultAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == today &&
                !x.nghiPhep);

            if (chamCong == null || chamCong.thoiGianRa != null)
            {
                TempData["Error"] = "Khong the check-out";
                return RedirectToAction("Index");
            }

            chamCong.thoiGianRa = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ===============================
        // 2. LỊCH SỬ CHẤM CÔNG (CALENDAR)
        // ===============================
        public IActionResult LichSu()
        {
            return View();
        }

        // API cho FullCalendar
        [HttpGet]
        public async Task<IActionResult> CalendarData(
            int month,
            int year,
            int? nhanVienId)
        {
            var user = await _userManager.GetUserAsync(User);

            // user thường chỉ xem chính mình
            if (!User.IsInRole("Admin"))
            {
                nhanVienId = user!.nhanVienId;
            }

            if (nhanVienId == null)
                return BadRequest();

            var chamCongs = await _context.ChamCongs
                .Where(x =>
                    x.nhanVienId == nhanVienId &&
                    x.thoiGianVao.Month == month &&
                    x.thoiGianVao.Year == year)
                .ToListAsync();

            var events = chamCongs.Select(x =>
            {
                var laCuoiTuan =
                    x.thoiGianVao.DayOfWeek == DayOfWeek.Saturday ||
                    x.thoiGianVao.DayOfWeek == DayOfWeek.Sunday;

                var soGio =
                    x.thoiGianRa.HasValue
                        ? (x.thoiGianRa.Value - x.thoiGianVao).TotalHours
                        : (double?)null;

                var diMuon = x.thoiGianVao.TimeOfDay > new TimeSpan(8, 0, 0);
                var veSom = x.thoiGianRa.HasValue &&
                            x.thoiGianRa.Value.TimeOfDay < new TimeSpan(17, 0, 0);

                var bg =
                    x.nghiPhep ? "#fff3cd" :
                    (diMuon || veSom) ? "#f8d7da" :
                    laCuoiTuan ? "#e2e3e5" :
                    "#d4edda";

                return new
                {
                    id = x.chamCongId,
                    start = x.thoiGianVao.Date,
                    title = soGio.HasValue ? $"{soGio:F1}h" : "",
                    backgroundColor = bg,
                    extendedProps = new
                    {
                        gioVao = x.thoiGianVao.ToString("HH:mm"),
                        gioRa = x.thoiGianRa?.ToString("HH:mm")
                    }
                };
            });

            return Json(events);
        }

        // ===============================
        // 3. CHI TIẾT 1 NGÀY
        // ===============================
        [HttpGet("ChamCong/ChiTiet/{id:int}")]
        public async Task<IActionResult> ChiTiet(int id)
        {
            var chamCong = await _context.ChamCongs
                .FirstOrDefaultAsync(x => x.chamCongId == id);

            if (chamCong == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // user thường không được xem người khác
            if (!User.IsInRole("Admin") &&
                chamCong.nhanVienId != user!.nhanVienId)
            {
                return Forbid();
            }

            var vm = new ChamCongChiTietVm
            {
                ChamCongId = chamCong.chamCongId,
                Ngay = chamCong.thoiGianVao.Date,
                GioVao = chamCong.thoiGianVao,
                GioRa = chamCong.thoiGianRa,
                NghiPhep = chamCong.nghiPhep,
                NghiPhepCoLuong = chamCong.nghiPhepCoLuong,
                AnTrua = chamCong.anTrua
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DanhSachNhanVien()
        {
            var nhanViens = await _context.NhanViens
                .Where(x => x.hoatDong)
                .OrderBy(x => x.hoTen)
                .Select(x => new
                {
                    id = x.nhanVienId,
                    text = x.hoTen
                })
                .ToListAsync();

            return Json(nhanViens);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetChamCongForEdit(int id)
        {
            var cc = await _context.ChamCongs
                .FirstOrDefaultAsync(x => x.chamCongId == id);

            if (cc == null)
                return NotFound();

            return Json(new SuaChamCongVm
            {
                ChamCongId = cc.chamCongId,
                ThoiGianVao = cc.thoiGianVao,
                ThoiGianRa = cc.thoiGianRa,
                NghiPhep = cc.nghiPhep,
                NghiPhepCoLuong = cc.nghiPhepCoLuong
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateChamCong([FromBody] SuaChamCongVm vm)
        {
            // ===== VALIDATE NGHIỆP VỤ =====
            // nghỉ phép mà vẫn có giờ → SAI
            if (vm.NghiPhep &&
                (vm.ThoiGianVao != default || vm.ThoiGianRa.HasValue))
            {
                return BadRequest("Nghi phep khong duoc co gio vao/ra");
            }

            // đi làm mà không có giờ vào → SAI
            if (!vm.NghiPhep && vm.ThoiGianVao == default)
            {
                return BadRequest("Di lam phai co gio vao");
            }

            if (!ModelState.IsValid)
                return BadRequest("Du lieu khong hop le");

            var chamCong = await _context.ChamCongs
                .FirstOrDefaultAsync(x => x.chamCongId == vm.ChamCongId);

            if (chamCong == null)
                return NotFound();

            // lấy adminNhanVienId từ user đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var adminUser = await _userManager.FindByIdAsync(userId);

            if (adminUser?.nhanVienId == null)
                return Forbid();

            var adminNhanVienId = adminUser.nhanVienId.Value;

            // ============================
            // 1. LƯU AUDIT (ĐÚNG MODEL)
            // ============================
            var audit = new ChamCongAudit
            {
                chamCongId = chamCong.chamCongId,
                adminNhanVienId = adminNhanVienId,

                // trạng thái cũ
                thoiGianVaoCu = chamCong.thoiGianVao,
                thoiGianRaCu = chamCong.thoiGianRa,
                nghiPhepCu = chamCong.nghiPhep,
                nghiPhepCoLuongCu = chamCong.nghiPhepCoLuong,

                // trạng thái mới
                thoiGianVaoMoi = vm.ThoiGianVao,
                thoiGianRaMoi = vm.ThoiGianRa,
                nghiPhepMoi = vm.NghiPhep,
                nghiPhepCoLuongMoi = vm.NghiPhepCoLuong,

                lyDo = vm.LyDo,
                suaLuc = DateTime.Now
            };

            // ============================
            // 2. UPDATE CHẤM CÔNG
            // ============================
            chamCong.thoiGianVao = vm.ThoiGianVao;
            chamCong.thoiGianRa = vm.ThoiGianRa;
            chamCong.nghiPhep = vm.NghiPhep;
            chamCong.nghiPhepCoLuong = vm.NghiPhepCoLuong;

            _context.ChamCongAudits.Add(audit);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("ChamCong/ChiTietTheoNgay")]
        public async Task<IActionResult> ChiTiet(DateTime ngay, int? nhanVienId)
        {
            // xác định nhân viên
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                nhanVienId = user!.nhanVienId;
            }

            if (nhanVienId == null)
                return BadRequest();

            var chamCong = await _context.ChamCongs.FirstOrDefaultAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao.Date == ngay.Date);

            if (chamCong == null)
            {
                // chưa có chấm công ngày này
                ViewBag.Ngay = ngay;
                ViewBag.NhanVienId = nhanVienId;
                return View("ChiTietTrong"); // view thông báo chưa có dữ liệu
            }

            return RedirectToAction("ChiTiet", new { id = chamCong.chamCongId });
        }

    }
}