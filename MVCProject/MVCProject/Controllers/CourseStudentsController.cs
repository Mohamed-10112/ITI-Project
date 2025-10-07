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
    public class CourseStudentsController : Controller
    {
        private readonly ICourseStudentsRepository _repository;
        private readonly AppDbContext _context;

        public CourseStudentsController(ICourseStudentsRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var list = string.IsNullOrEmpty(searchString)
                ? _repository.GetAll()
                : _repository.Search(searchString);

            return View(list);
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var cs = _repository.GetById(id.Value);
            if (cs == null) return NotFound();

            return View(cs);
        }

        public IActionResult Create()
        {
            ViewData["CrsId"] = new SelectList(_repository.GetCourses(), "Id", "Name");
            ViewData["StdId"] = new SelectList(_repository.GetStudents(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CourseStudents courseStudents)
        {
            ModelState.Remove("Course");
            ModelState.Remove("Student");

            if (ModelState.IsValid)
            {
                _repository.Add(courseStudents);
                return RedirectToAction(nameof(Index));
            }

            ViewData["CrsId"] = new SelectList(_repository.GetCourses(), "Id", "Name", courseStudents.CrsId);
            ViewData["StdId"] = new SelectList(_repository.GetStudents(), "Id", "Name", courseStudents.StdId);
            return View(courseStudents);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var cs = _repository.GetById(id.Value);
            if (cs == null) return NotFound();

            ViewData["CrsId"] = new SelectList(_repository.GetCourses(), "Id", "Name", cs.CrsId);
            ViewData["StdId"] = new SelectList(_repository.GetStudents(), "Id", "Name", cs.StdId);
            return View(cs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CourseStudents courseStudents)
        {
            ModelState.Remove("Course");
            ModelState.Remove("Student");

            if (id != courseStudents.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(courseStudents);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_repository.GetById(courseStudents.Id) == null)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CrsId"] = new SelectList(_repository.GetCourses(), "Id", "Name", courseStudents.CrsId);
            ViewData["StdId"] = new SelectList(_repository.GetStudents(), "Id", "Name", courseStudents.StdId);
            return View(courseStudents);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var cs = _repository.GetById(id.Value);
            if (cs == null) return NotFound();

            return View(cs);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleCompletion(int id)
        {
            var courseStudent = _context.CourseStudents.FirstOrDefault(cs => cs.Id == id);
            if (courseStudent == null)
                return NotFound();

            courseStudent.IsCompleted = !courseStudent.IsCompleted;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}
