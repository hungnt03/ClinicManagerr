using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
