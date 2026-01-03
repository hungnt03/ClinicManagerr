using System.Diagnostics;
using ClinicManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Admin → Dashboard
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            // Bacsy → Dashboard
            if (User.IsInRole("BacSi") || User.IsInRole("KyThuatVien"))
            {
                return RedirectToAction("BacSi", "Dashboard");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
