using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Models.Entities;
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
            var tomorrow = today.AddDays(1);

            var banGhiHienTai = await _context.ChamCongs.FirstOrDefaultAsync(x =>
                x.nhanVienId == nhanVienId &&
                x.thoiGianVao >= today && x.thoiGianVao < tomorrow);

            if (banGhiHienTai != null)
            {
                TempData["Error"] = banGhiHienTai.nghiPhep ? "Hôm nay bạn đã đăng ký nghỉ phép." : "Bạn đã chấm công vào hôm nay.";
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

        [HttpGet]
        public async Task<IActionResult> CalendarData(
    int month,
    int year,
    int? nhanVienId)
        {
            var user = await _userManager.GetUserAsync(User);

            // 1. Phân quyền: User thường chỉ xem chính mình
            if (!User.IsInRole("Admin"))
            {
                nhanVienId = user?.nhanVienId;
            }

            if (nhanVienId == null)
                return BadRequest();

            // 2. Lấy dữ liệu từ Database
            var chamCongs = await _context.ChamCongs
                .Where(x =>
                    x.nhanVienId == nhanVienId &&
                    x.thoiGianVao.Month == month &&
                    x.thoiGianVao.Year == year)
                .ToListAsync();

            // 3. Mapping dữ liệu sang format FullCalendar
            var events = chamCongs.Select(x =>
            {
                // --- LOGIC KIỂM TRA NGHIỆP VỤ ---
                var gioVaoChuan = new TimeSpan(8, 5, 0);  // Cho phép trễ 5 phút
                var gioRaChuan = new TimeSpan(17, 0, 0);

                var laCuoiTuan = x.thoiGianVao.DayOfWeek == DayOfWeek.Saturday ||
                                 x.thoiGianVao.DayOfWeek == DayOfWeek.Sunday;

                var diMuon = !x.nghiPhep && x.thoiGianVao.TimeOfDay > gioVaoChuan;

                var veSom = !x.nghiPhep && x.thoiGianRa.HasValue &&
                            x.thoiGianRa.Value.TimeOfDay < gioRaChuan;

                // Quên checkout: Không nghỉ phép, không có giờ ra và đã qua ngày hôm đó
                var quenCheckOut = !x.nghiPhep && !x.thoiGianRa.HasValue &&
                                   x.thoiGianVao.Date < DateTime.Today;

                var soGio = x.thoiGianRa.HasValue
                        ? (x.thoiGianRa.Value - x.thoiGianVao).TotalHours
                        : (double?)null;

                // --- THIẾT LẬP MÀU SẮC THEO YÊU CẦU ---
                string bg = "#d4edda"; // Mặc định: Xanh nhạt (Làm đủ/Tốt)
                string textColor = "#155724";

                if (x.nghiPhep)
                {
                    bg = "#e2e3e5"; // Xám (Nghỉ phép)
                    textColor = "#383d41";
                }
                else if (quenCheckOut)
                {
                    bg = "#f8d7da"; // Đỏ nhạt (Cảnh báo quên Checkout)
                    textColor = "#721c24";
                }
                else if (diMuon || veSom)
                {
                    bg = "#fff3cd"; // Vàng (Đi muộn hoặc về sớm)
                    textColor = "#856404";
                }
                else if (laCuoiTuan && !soGio.HasValue)
                {
                    bg = "#f8f9fa"; // Xám rất nhạt cho cuối tuần không đi làm
                    textColor = "#6c757d";
                }

                // --- TRẢ VỀ OBJECT CHO FRONTEND ---
                return new
                {
                    id = x.chamCongId,
                    // Sử dụng format yyyy-MM-dd để tránh lệch múi giờ JS
                    start = x.thoiGianVao.ToString("yyyy-MM-dd"),
                    title = x.nghiPhep ? "NGHỈ" : (soGio.HasValue ? $"{soGio:F1}h" : "Thiếu giờ"),
                    backgroundColor = bg,
                    borderColor = bg,
                    textColor = textColor,
                    extendedProps = new
                    {
                        gioVao = x.thoiGianVao.ToString("HH:mm"),
                        gioRa = x.thoiGianRa?.ToString("HH:mm") ?? "--:--",
                        anTrua = x.anTrua ? "Có" : "Không",
                        trangThai = x.nghiPhep ? "Nghỉ phép" : (diMuon ? "Đi muộn" : (veSom ? "Về sớm" : "Bình thường"))
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
                return BadRequest("Nghỉ phép không được có giờ vào / ra");
            }

            // đi làm mà không có giờ vào → SAI
            if (!vm.NghiPhep && vm.ThoiGianVao == default)
            {
                return BadRequest("Đi làm phải có giờ vào");
            }

            if (!vm.NghiPhep)
            {
                if (vm.ThoiGianRa.HasValue && vm.ThoiGianRa <= vm.ThoiGianVao)
                {
                    return BadRequest("Giờ ra phải sau giờ vào");
                }
            }

            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ");

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