using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCProject.Models;
using MVCProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCProject.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IInstructorRepository _instructorRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public InstructorsController(IInstructorRepository instructorRepository, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _instructorRepository = instructorRepository;
            _userManager = userManager;
            _context = context;
        }


        public IActionResult Index(string searchString)
        {
            var instructors = string.IsNullOrEmpty(searchString)
                ? _instructorRepository.GetAll()
                : _instructorRepository.Search(searchString);

            return View(instructors);
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var instructor = _instructorRepository.GetById(id.Value);
            if (instructor == null) return NotFound();

            return View(instructor);
        }

        public IActionResult Create()
        {
            ViewData["CrsId"] = new SelectList(_instructorRepository.GetCourses(), "Id", "Name");
            ViewData["DeptId"] = new SelectList(_instructorRepository.GetDepartments(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Instructor instructor)
        {
            ModelState.Remove("Department");
            ModelState.Remove("Course");

            if (ModelState.IsValid)
            {
                _instructorRepository.Add(instructor);
                return RedirectToAction(nameof(Index));
            }

            ViewData["CrsId"] = new SelectList(_instructorRepository.GetCourses(), "Id", "Name", instructor.CrsId);
            ViewData["DeptId"] = new SelectList(_instructorRepository.GetDepartments(), "Id", "Name", instructor.DeptId);
            return View(instructor);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var instructor = _instructorRepository.GetById(id.Value);
            if (instructor == null) return NotFound();

            ViewData["CrsId"] = new SelectList(_instructorRepository.GetCourses(), "Id", "Name", instructor.CrsId);
            ViewData["DeptId"] = new SelectList(_instructorRepository.GetDepartments(), "Id", "Name", instructor.DeptId);
            return View(instructor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Instructor instructor)
        {
            ModelState.Remove("Department");
            ModelState.Remove("Course");
            ModelState.Remove("User");

            if (id != instructor.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var instructorFromDb = _instructorRepository.GetById(id);
                    if (instructorFromDb == null)
                        return NotFound();

                    instructor.UserId = instructorFromDb.UserId;

                    instructorFromDb.Name = instructor.Name;
                    instructorFromDb.Address = instructor.Address;
                    instructorFromDb.Salary = instructor.Salary;
                    instructorFromDb.DeptId = instructor.DeptId;
                    instructorFromDb.CrsId = instructor.CrsId;
                    instructorFromDb.Image = instructor.Image;

                    _instructorRepository.Update(instructorFromDb);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_instructorRepository.GetById(instructor.Id) == null)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CrsId"] = new SelectList(_instructorRepository.GetCourses(), "Id", "Name", instructor.CrsId);
            ViewData["DeptId"] = new SelectList(_instructorRepository.GetDepartments(), "Id", "Name", instructor.DeptId);
            return View(instructor);
        }


        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var instructor = _instructorRepository.GetById(id.Value);
            if (instructor == null) return NotFound();

            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = _instructorRepository.GetById(id);
            if (instructor != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == instructor.UserId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                _instructorRepository.Delete(id);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> MyCourse()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var instructor = await _context.Instructors
                .Include(i => i.Course)
                    .ThenInclude(c => c.CourseStudents)
                        .ThenInclude(cs => cs.Student)
                            .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            if (instructor == null)
                return NotFound("Instructor not found for this user.");

            return View(instructor);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateGrade(int courseId, int studentId, int grade)
        {
            var courseStudent = await _context.CourseStudents
                .FirstOrDefaultAsync(cs => cs.CrsId == courseId && cs.StdId == studentId);

            if (courseStudent == null)
                return NotFound();

            courseStudent.Degree = grade;
            await _context.SaveChangesAsync();

            return RedirectToAction("MyCourse");
        }


    }
}
