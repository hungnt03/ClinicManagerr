using ClinicManager.Data;
using ClinicManager.Services;
using ClinicManager.ViewModels.VatTu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Controllers
{
    [Authorize]
    public class VatTuController : Controller
    {

        private readonly IVatTuService _vatTuService;
        private readonly ApplicationDbContext _context;

        public VatTuController(ApplicationDbContext context,IVatTuService vatTuService)
        {
            _context = context;
            _vatTuService = vatTuService;
        }

        // ==================================================
        // GET ALL – DÙNG CHO MODAL THÊM VẬT TƯ
        // ==================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.VatTus
                .Where(x => x.tonKho > 0)              // chỉ lấy còn tồn
                .OrderBy(x => x.tenVatTu)
                .Select(x => new
                {
                    vatTuId = x.vatTuId,
                    tenVatTu = x.tenVatTu,
                    tonKho = x.tonKho,
                    donGia = x.donGia
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(Array.Empty<object>());

            var data = await _context.VatTus
                .Where(x => x.tonKho > 0 && x.tenVatTu.Contains(term))
                .OrderBy(x => x.tenVatTu)
                .Take(20)
                .Select(x => new
                {
                    id = x.vatTuId,
                    text = x.tenVatTu + " (" + x.tonKho + ")",
                    donGia = x.donGia
                })
                .ToListAsync();

            return Json(data);
        }

        public async Task<IActionResult> Index(
        string? keyword,
        int page = 1)
        {
            const int pageSize = 10;

            var vm = await _vatTuService.SearchAsync(
                keyword,
                page,
                pageSize);

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new VatTuCreateEditVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VatTuCreateEditVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _vatTuService.CreateAsync(vm);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _vatTuService.GetByIdAsync(id);
            if (vm == null) return NotFound();

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VatTuCreateEditVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _vatTuService.UpdateAsync(vm);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
            }
            return View(vm);
        }
    }
}