using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MVCProject.Models;

namespace MVCProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseRequestsController : Controller
    {
        private readonly AppDbContext _context;

        public CourseRequestsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var requests = _context.CourseRequests
                .Include(r => r.Student)
                .Include(r => r.Course)
                .OrderByDescending(r => r.RequestDate)
                .ToList();

            return View(requests);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var request = _context.CourseRequests
                .Include(r => r.Student)
                .Include(r => r.Course)
                .FirstOrDefault(r => r.Id == id);

            if (request == null) return NotFound();

            var newEnrollment = new CourseStudents
            {
                StdId = request.StudentId,
                CrsId = request.CourseId,
                IsCompleted = false,
                Degree = 0
            };

            _context.CourseStudents.Add(newEnrollment);

            request.Status = "Approved";
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var request = _context.CourseRequests.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            request.Status = "Rejected";
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
