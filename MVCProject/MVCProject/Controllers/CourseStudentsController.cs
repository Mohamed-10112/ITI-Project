using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCProject.Models;

namespace MVCProject.Controllers
{
    public class CourseStudentsController : Controller
    {
        private readonly AppDbContext _context;

        public CourseStudentsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var appDbContext = _context.CourseStudents.Include(c => c.Course).Include(c => c.Student).ToList();
            return View(appDbContext);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = _context.CourseStudents
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefault(m => m.Id == id);
            if (courseStudents == null)
            {
                return NotFound();
            }

            return View(courseStudents);
        }

        public IActionResult Create()
        {
            ViewData["CrsId"] = new SelectList(_context.Courses, "Id", "Name");
            ViewData["StdId"] = new SelectList(_context.Students, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Degree,CrsId,StdId")] CourseStudents courseStudents)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseStudents);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CrsId"] = new SelectList(_context.Courses, "Id", "Name", courseStudents.CrsId);
            ViewData["StdId"] = new SelectList(_context.Students, "Id", "Name", courseStudents.StdId);
            return View(courseStudents);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = _context.CourseStudents.Find(id);
            if (courseStudents == null)
            {
                return NotFound();
            }
            ViewData["CrsId"] = new SelectList(_context.Courses, "Id", "Name", courseStudents.CrsId);
            ViewData["StdId"] = new SelectList(_context.Students, "Id", "Name", courseStudents.StdId);
            return View(courseStudents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Degree,CrsId,StdId")] CourseStudents courseStudents)
        {
            if (id != courseStudents.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseStudents);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseStudentsExists(courseStudents.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CrsId"] = new SelectList(_context.Courses, "Id", "Name", courseStudents.CrsId);
            ViewData["StdId"] = new SelectList(_context.Students, "Id", "Name", courseStudents.StdId);
            return View(courseStudents);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseStudents = _context.CourseStudents
                .Include(c => c.Course)
                .Include(c => c.Student)
                .FirstOrDefault(m => m.Id == id);
            if (courseStudents == null)
            {
                return NotFound();
            }

            return View(courseStudents);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var courseStudents = _context.CourseStudents.Find(id);
            if (courseStudents != null)
            {
                _context.CourseStudents.Remove(courseStudents);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseStudentsExists(int id)
        {
            return _context.CourseStudents.Any(e => e.Id == id);
        }
    }
}
