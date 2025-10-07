using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCProject.Models;
using MVCProject.ViewModels;

namespace MVCProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("ManageUsers");
        }

        public IActionResult ManageUsers()
        {
            var users = userManager.Users.ToList();
            return View("ManageUsers", users);
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = roleManager.Roles.Select(r => r.Name).ToList();
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new AssignRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                SelectedRole = userRoles.FirstOrDefault(),
                Roles = roles
            };

            return View("AssignRole", model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model, [FromServices] AppDbContext context)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, userRoles);
            await userManager.AddToRoleAsync(user, model.SelectedRole);

            if (model.SelectedRole == "Student")
            {
                if (!context.Students.Any(s => s.UserId == user.Id))
                {
                    var student = new Student
                    {
                        Name = user.DisplayName,
                        UserId = user.Id,
                        Grade = 0,
                        DeptId = 7
                    };
                    context.Students.Add(student);
                    await context.SaveChangesAsync();
                }
            }

            else if (model.SelectedRole == "Instructor")
            {
                if (!context.Instructors.Any(i => i.UserId == user.Id))
                {
                    var instructor = new Instructor
                    {
                        Name = "Dr. " + user.DisplayName,
                        UserId = user.Id,
                        DeptId = 1,
                        CrsId = 1,
                        Salary = 0
                    };
                    context.Instructors.Add(instructor);
                    await context.SaveChangesAsync();
                }
            }

            //TempData["SuccessMessage"] = $"Role '{model.SelectedRole}' assigned to {user.Email}";
            return RedirectToAction("ManageUsers");
        }

    }
}
