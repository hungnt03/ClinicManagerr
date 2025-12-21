using ClinicManager.Data;
using ClinicManager.Models.Entities;
using ClinicManager.ViewModels.Luong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class NgayLeController : Controller
{
    private readonly ApplicationDbContext _context;

    public NgayLeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _context.NgayLes
            .OrderBy(x => x.ngay)
            .Select(x => new NgayLeVm
            {
                NgayLeId = x.ngayLeId,
                Ngay = x.ngay,
                Ten = x.ten,
                CoTinhLuong = x.coTinhLuong
            })
            .ToListAsync();

        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new NgayLeVm { Ngay = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NgayLeVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        _context.NgayLes.Add(new NgayLe
        {
            ngay = vm.Ngay,
            ten = vm.Ten,
            coTinhLuong = vm.CoTinhLuong,
            taoLuc = DateTime.Now
        });

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
