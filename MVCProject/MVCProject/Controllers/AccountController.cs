using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCProject.Models;
using MVCProject.ViewModels;
using System.Threading.Tasks;

namespace MVCProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        private string? GetFormValue(params string[] keys)
        {
            foreach (var k in keys)
            {
                if (Request.Form.TryGetValue(k, out var val))
                {
                    var s = val.ToString();
                    if (!string.IsNullOrWhiteSpace(s)) return s;
                }
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> SaveRegister()
        {
            var firstName = GetFormValue("Register.FirstName", "FirstName");
            var lastName = GetFormValue("Register.LastName", "LastName");
            var email = GetFormValue("Register.Email", "Email");
            var password = GetFormValue("Register.Password", "Password");
            var confirm = GetFormValue("Register.ConfirmPassword", "ConfirmPassword");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return RedirectToAction("Index", "Home", new
                {
                    showRegister = true,
                    email = email ?? "",
                    firstName = firstName ?? "",
                    lastName = lastName ?? "",
                    error = "Please fill required fields."
                });
            }

            if (password != confirm)
            {
                return RedirectToAction("Index", "Home", new
                {
                    showRegister = true,
                    email,
                    firstName,
                    lastName,
                    error = "Password and Confirm Password do not match."
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return RedirectToAction("Index", "Home", new
                {
                    showRegister = true,
                    email,
                    firstName,
                    lastName,
                    error = "This email is already registered."
                });
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home", new
                {
                    showLogin = true,
                    email,
                    success = "Account created successfully. Please wait for admin approval."
                });
            }

            var errorMessages = string.Join(" ", result.Errors.Select(e => e.Description));
            return RedirectToAction("Index", "Home", new
            {
                showRegister = true,
                email,
                firstName,
                lastName,
                error = errorMessages
            });
        }

        [HttpPost]
        public async Task<IActionResult> SaveLogin()
        {
            var email = GetFormValue("Login.Email", "Email");
            var password = GetFormValue("Login.Password", "Password");
            var rememberVal = GetFormValue("Login.RememberMe", "RememberMe", "rememberMe");

            bool remember = false;
            if (!string.IsNullOrEmpty(rememberVal))
            {
                if (bool.TryParse(rememberVal, out var tmp)) remember = tmp;
                else if (rememberVal.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                         rememberVal.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                         rememberVal.Equals("1")) remember = true;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return RedirectToAction("Index", "Home", new
                {
                    showLogin = true,
                    email = email ?? "",
                    error = "Please fill required fields."
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("Index", "Home", new
                {
                    showLogin = true,
                    email,
                    error = "Invalid Email or Password!"
                });
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, password, remember, false);

            if (!signInResult.Succeeded)
            {
                return RedirectToAction("Index", "Home", new
                {
                    showLogin = true,
                    email,
                    error = "Invalid Email or Password!"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count == 0)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home", new
                {
                    showLogin = true,
                    email,
                    error = "Your account is not yet approved by the admin."
                });
            }

            if (roles.Contains("Admin")) return RedirectToAction("Index", "Students");
            if (roles.Contains("Instructor")) return RedirectToAction("MyCourse", "Instructors");
            if (roles.Contains("Student")) return RedirectToAction("MyCourses", "Students");
            if (roles.Contains("HR")) return RedirectToAction("Index", "Students");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

