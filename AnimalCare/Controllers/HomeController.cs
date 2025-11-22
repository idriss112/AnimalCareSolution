using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AnimalCare.Controllers
{
    // Home pages – mostly public
    public class HomeController : Controller
    {
        // Public home page – no login required
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        // Privacy  page
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy Policy";
            return View();
        }

        // Example public page – About
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        // Example public page – Contact
        [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }

        // Access denied page – used when user is authenticated but not allowed (wrong role)
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            // You will create Views/Home/AccessDenied.cshtml
            return View();
        }
    }
}
