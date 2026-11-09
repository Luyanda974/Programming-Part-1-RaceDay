using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services; 

namespace RaceDay.Web.Controllers
{//start of namespace
    public class AuthController : Controller
    {//start of class
        private readonly RaceDayApiService _apiService;

        // Receive the API service through dependency injection.
        public AuthController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        // Display the registration page.
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Send registration details to the RaceDay API.
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var response = await _apiService.PostAsync<RegisterRequest, RegisterResponse>(
                    "api/Auth/register", request);

                TempData["Success"] = "Registration successful! Please log in.";

                return RedirectToAction("Login");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Registration failed. Please check your details.");
                return View(request);
            }
        }

        // Display the login page.
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Send login details to the RaceDay API.
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var response = await _apiService.PostAsync<LoginRequest, AuthResponse>(
                    "api/Auth/login", request);

                if (response == null || string.IsNullOrEmpty(response.Token))
                {
                    ModelState.AddModelError("", "Invalid login response.");
                    return View(request);
                }

                // Store the JWT token and user details in the session.
                HttpContext.Session.SetString("JWToken", response.Token);
                HttpContext.Session.SetString("UserID", response.UserID.ToString());
                HttpContext.Session.SetString("FirstName", response.FirstName);
                HttpContext.Session.SetString("LastName", response.LastName);
                HttpContext.Session.SetString("Email", response.Email);
                HttpContext.Session.SetString("Role", response.Role);

                // Redirect the user to the home page after login.
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(request);
            }
        }

        // Log out the user and clear their session.
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }//end of class
}//end of namepsace
