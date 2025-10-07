using Microsoft.AspNetCore.Mvc;
using MVCProject.Models;
using MVCProject.ViewModels;
using System.Diagnostics;

namespace MVCProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(bool? showLogin, bool? showRegister, string? email, string? firstName, string? lastName, string? error, string? success)
        {
            var model = new HomePageViewModel
            {
                Login = new LoginUserViewModel(),
                Register = new RegisterUserViewModel()
            };

            if (showLogin == true) ViewBag.ShowLogin = true;
            if (showRegister == true) ViewBag.ShowRegister = true;

            if (!string.IsNullOrEmpty(email)) ViewBag.LoginEmail = email;
            if (!string.IsNullOrEmpty(firstName)) ViewBag.RegisterFirstName = firstName;
            if (!string.IsNullOrEmpty(lastName)) ViewBag.RegisterLastName = lastName;

            if (!string.IsNullOrEmpty(error)) ViewBag.ErrorMessage = error;
            if (!string.IsNullOrEmpty(success)) ViewBag.SuccessMessage = success;

            if (TempData["ShowLoginPopup"] != null) ViewBag.ShowLogin = TempData["ShowLoginPopup"];
            if (TempData["ShowRegisterPopup"] != null) ViewBag.ShowRegister = TempData["ShowRegisterPopup"];
            if (TempData["Email"] != null && string.IsNullOrEmpty((string?)ViewBag.LoginEmail)) ViewBag.LoginEmail = TempData["Email"];
            if (TempData["FirstName"] != null && string.IsNullOrEmpty((string?)ViewBag.RegisterFirstName)) ViewBag.RegisterFirstName = TempData["FirstName"];
            if (TempData["LastName"] != null && string.IsNullOrEmpty((string?)ViewBag.RegisterLastName)) ViewBag.RegisterLastName = TempData["LastName"];
            if (TempData["ErrorMessage"] != null && string.IsNullOrEmpty((string?)ViewBag.ErrorMessage)) ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (TempData["SuccessMessage"] != null && string.IsNullOrEmpty((string?)ViewBag.SuccessMessage)) ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(model);
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
