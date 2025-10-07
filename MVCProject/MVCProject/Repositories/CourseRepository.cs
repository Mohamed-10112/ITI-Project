using MVCProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MVCProject.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Course> GetAll()
        {
            return _context.Courses.Include(c => c.Department).ToList();
        }

        public IEnumerable<Course> SearchByName(string searchString)
        {
            return _context.Courses
                           .Include(c => c.Department)
                           .Where(c => c.Name.Contains(searchString))
                           .ToList();
        }

        public Course GetById(int id)
        {
            return _context.Courses.Include(c => c.Department).FirstOrDefault(c => c.Id == id);
        }

        public void Add(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id)
        {
            return _context.Courses.Any(c => c.Id == id);
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _context.Departments.ToList();
        }
    }
}
