using System.Security.Claims;
using ClinicManager.Models;
using ClinicManager.Services;
using ClinicManager.ViewModels.NhapKho;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class NhapKhoController : Controller
{
    private readonly INhapKhoService _nhapKhoService;
    private readonly UserManager<ApplicationUser> _userManager;

    public NhapKhoController(INhapKhoService nhapKhoService, UserManager<ApplicationUser> userManager)
    {
        _nhapKhoService = nhapKhoService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new NhapKhoCreateVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhapKhoCreateVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var adminUser = await _userManager.FindByIdAsync(userId);

        if (adminUser?.nhanVienId == null)
            return Forbid();

        var nhanVienId = adminUser.nhanVienId.Value;

        await _nhapKhoService.TaoPhieuNhapAsync(vm, nhanVienId);

        return RedirectToAction("Index", "VatTu");
    }
}
