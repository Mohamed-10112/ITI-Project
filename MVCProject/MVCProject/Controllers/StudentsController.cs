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
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public StudentsController(IStudentRepository studentRepository, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _studentRepository = studentRepository;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var students = string.IsNullOrWhiteSpace(searchString)
                ? _studentRepository.GetAll()
                : _studentRepository.Search(searchString);

            return View(students);
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var student = _studentRepository.GetById(id.Value);
            if (student == null) return NotFound();

            return View(student);
        }

        public IActionResult Create()
        {
            ViewData["DeptId"] = new SelectList(_studentRepository.GetDepartments(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student, IFormFile ImageFile)
        {
            ModelState.Remove("Image");
            ModelState.Remove("ImageFile");
            ModelState.Remove("Department");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                student.Image = SaveImage(ImageFile);
                _studentRepository.Add(student);
                return RedirectToAction(nameof(Index));
            }

            ViewData["DeptId"] = new SelectList(_studentRepository.GetDepartments(), "Id", "Name", student.DeptId);
            return View(student);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = _studentRepository.GetById(id.Value);
            if (student == null) return NotFound();

            ViewData["DeptId"] = new SelectList(_studentRepository.GetDepartments(), "Id", "Name", student.DeptId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Student student, IFormFile ImageFile)
        {
            if (id != student.Id) return NotFound();

            ModelState.Remove("Image");
            ModelState.Remove("ImageFile");
            ModelState.Remove("Department");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                var studentFromDb = _studentRepository.GetById(id);
                if (studentFromDb == null) return NotFound();

                studentFromDb.Name = student.Name;
                studentFromDb.Address = student.Address;
                studentFromDb.Grade = student.Grade;
                studentFromDb.DeptId = student.DeptId;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    studentFromDb.Image = SaveImage(ImageFile);
                }

                _studentRepository.Update(studentFromDb);
                return RedirectToAction(nameof(Index));
            }

            ViewData["DeptId"] = new SelectList(_studentRepository.GetDepartments(), "Id", "Name", student.DeptId);
            return View(student);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = _studentRepository.GetById(id.Value);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = _studentRepository.GetById(id);
            if (student != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == student.UserId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                if (!string.IsNullOrEmpty(student.Image) && student.Image != "default.jpg")
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/students", student.Image);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _studentRepository.Delete(id);
            }

            return RedirectToAction(nameof(Index));
        }


        private string SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return "default.jpg";

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/students");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var path = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return fileName;
        }

        [Authorize(Roles = "Student")]
        public IActionResult MyProfile()
        {
            var userId = _userManager.GetUserId(User);

            var student = _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null)
                return NotFound();

            return View("StudentProfile", student);
        }

        [Authorize(Roles = "Student")]
        public IActionResult MyCourses()
        {
            var userId = _userManager.GetUserId(User);

            var student = _context.Students
                .Include(s => s.CourseStudents)
                    .ThenInclude(cs => cs.Course)
                        .ThenInclude(c => c.Department)
                .Include(s => s.CourseStudents)
                    .ThenInclude(cs => cs.Course)
                        .ThenInclude(c => c.Instructors)
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null)
                return NotFound("Student profile not found.");

            var myCourses = student.CourseStudents
                .Where(cs => !cs.IsCompleted)
                .Select(cs => new
                {
                    CourseName = cs.Course.Name,
                    Hours = cs.Course.Hours,
                    Department = cs.Course.Department.Name,
                    Instructor = cs.Course.Instructors.FirstOrDefault()?.Name ?? "N/A"
                }).ToList();

            ViewBag.StudentName = student.Name;
            return View(myCourses);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Grades()
        {
            var userId = _userManager.GetUserId(User);

            var student = _context.Students
                .Include(s => s.CourseStudents)
                    .ThenInclude(cs => cs.Course)
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null)
                return NotFound("Student profile not found.");

            var grades = student.CourseStudents
                .Where(cs => cs.IsCompleted)
                .Select(cs => new
                {
                    CourseName = cs.Course.Name,
                    Hours = cs.Course.Hours,
                    CourseDegree = cs.Course.Degree,
                    StudentDegree = cs.Degree,
                    MinimumDegree = cs.Course.MinimumDegree,
                    Status = cs.Degree >= cs.Course.MinimumDegree ? "Passed" : "Failed"
                }).ToList();

            ViewBag.StudentName = student.Name;
            return View(grades);
        }

        // for available courses
        [Authorize(Roles = "Student")]
        public IActionResult AvailableCourses()
        {
            var userId = _userManager.GetUserId(User);

            var student = _context.Students
                .Include(s => s.CourseStudents)
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null)
                return NotFound("Student profile not found.");

            var enrolledCourseIds = student.CourseStudents.Select(cs => cs.CrsId).ToList();

            var availableCourses = _context.Courses
                .Include(c => c.Department)
                .Where(c => !enrolledCourseIds.Contains(c.Id))
                .ToList();

            var pendingRequests = _context.CourseRequests
                .Where(r => r.StudentId == student.Id && r.Status == "Pending")
                .Select(r => r.CourseId)
                .ToList();

            ViewBag.PendingRequests = pendingRequests;

            return View(availableCourses);
        }


        [Authorize(Roles = "Student")]
        [HttpPost]
        public IActionResult RequestCourse(int courseId)
        {
            var userId = _userManager.GetUserId(User);
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);

            if (student == null)
                return NotFound("Student profile not found.");

            var existingRequest = _context.CourseRequests
                .FirstOrDefault(r => r.StudentId == student.Id && r.CourseId == courseId && r.Status == "Pending");

            if (existingRequest != null)
            {
                TempData["Message"] = "You already requested this course.";
                return RedirectToAction("AvailableCourses");
            }

            var newRequest = new CourseRequest
            {
                StudentId = student.Id,
                CourseId = courseId,
                Status = "Pending"
            };

            _context.CourseRequests.Add(newRequest);
            _context.SaveChanges();

            TempData["Message"] = "Course request sent successfully!";
            return RedirectToAction("AvailableCourses");
        }

    }
}
