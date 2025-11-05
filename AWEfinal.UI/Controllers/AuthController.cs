using Microsoft.AspNetCore.Mvc;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;

namespace AWEfinal.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and password are required";
                _logger.LogWarning("Login attempt with empty email or password");
                return View();
            }

            try
            {
                _logger.LogInformation($"Login attempt for email: {email}");
                var user = await _userService.LoginUserAsync(email, password);
                
                if (user == null)
                {
                    ViewBag.Error = "Invalid email or password. Please check your credentials and try again.";
                    _logger.LogWarning($"Login failed for email: {email}");
                    return View();
                }

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                _logger.LogInformation($"Login successful for user: {user.Email} (Role: {user.Role})");

                if (user.Role == "admin")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for email: {email}");
                ViewBag.Error = "An error occurred during login. Please try again.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string name)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
            {
                ViewBag.Error = "All fields are required";
                return View();
            }

            try
            {
                var user = await _userService.RegisterUserAsync(email, password, name);
                
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "admin";
        }
    }
}

