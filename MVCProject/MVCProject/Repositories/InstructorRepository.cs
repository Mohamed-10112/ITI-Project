using MVCProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MVCProject.Repositories
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly AppDbContext _context;

        public InstructorRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Instructor> GetAll()
        {
            return _context.Instructors
                           .Include(i => i.Course)
                           .Include(i => i.Department)
                           .ToList();
        }

        public IEnumerable<Instructor> Search(string searchString)
        {
            var q = searchString.Trim().ToLower();
            return _context.Instructors
                           .Include(i => i.Course)
                           .Include(i => i.Department)
                           .Where(i => i.Name.ToLower().Contains(q) ||
                                       (i.Course != null && i.Course.Name.ToLower().Contains(q)) ||
                                       (i.Department != null && i.Department.Name.ToLower().Contains(q)))
                           .ToList();
        }

        public Instructor GetById(int id)
        {
            return _context.Instructors
                           .Include(i => i.Course)
                           .Include(i => i.Department)
                           .FirstOrDefault(i => i.Id == id);
        }

        public void Add(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            _context.SaveChanges();
        }

        public void Update(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var instructor = _context.Instructors.Find(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Course> GetCourses()
        {
            return _context.Courses.ToList();
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _context.Departments.ToList();
        }
    }
}
