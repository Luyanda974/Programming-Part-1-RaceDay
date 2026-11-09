using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Diagnostics;

namespace RaceDay.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Display the home page after checking the user's session.
        public IActionResult Index()
        {
            // Check whether a user is logged in.
            string? userID = HttpContext.Session.GetString("UserID");

            // Redirect users who are not logged in.
            if (string.IsNullOrEmpty(userID))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Retrieve the logged-in user's information.
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
