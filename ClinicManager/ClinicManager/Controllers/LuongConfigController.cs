using ClinicManager.Services;
using ClinicManager.ViewModels.Luong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class LuongConfigController : Controller
{
    private readonly ILuongConfigService _service;

    public LuongConfigController(ILuongConfigService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _service.GetAllAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CauHinhLuongVm
        {
            ApDungTuNgay = DateTime.Today
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CauHinhLuongVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        await _service.CreateAsync(vm);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id)
    {
        var vm = await _service.GetByIdAsync(id);
        if (vm == null) return NotFound();

        return View(vm);
    }

}
