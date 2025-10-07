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
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;

        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public IActionResult Index(string searchString)
        {
            var courses = string.IsNullOrEmpty(searchString)
                ? _courseRepository.GetAll()
                : _courseRepository.SearchByName(searchString);

            return View(courses);
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var course = _courseRepository.GetById(id.Value);
            if (course == null) return NotFound();

            return View(course);
        }

        public IActionResult Create()
        {
            ViewData["DeptId"] = new SelectList(_courseRepository.GetDepartments(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            ModelState.Remove("Department");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage);
                return Content("Errors: " + string.Join(", ", errors));
            }

            _courseRepository.Add(course);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = _courseRepository.GetById(id.Value);
            if (course == null) return NotFound();

            ViewData["DeptId"] = new SelectList(_courseRepository.GetDepartments(), "Id", "Name", course.DeptId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Course course)
        {
            ModelState.Remove("Department");
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _courseRepository.Update(course);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_courseRepository.Exists(course.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["DeptId"] = new SelectList(_courseRepository.GetDepartments(), "Id", "Name", course.DeptId);
            return View(course);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = _courseRepository.GetById(id.Value);
            if (course == null) return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _courseRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
