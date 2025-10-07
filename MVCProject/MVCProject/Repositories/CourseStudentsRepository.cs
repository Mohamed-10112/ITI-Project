using MVCProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MVCProject.Repositories
{
    public class CourseStudentsRepository : ICourseStudentsRepository
    {
        private readonly AppDbContext _context;

        public CourseStudentsRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CourseStudents> GetAll()
        {
            return _context.CourseStudents
                           .Include(cs => cs.Course)
                           .Include(cs => cs.Student)
                           .ToList();
        }

        public IEnumerable<CourseStudents> Search(string searchString)
        {
            var q = searchString.Trim().ToLower();
            return _context.CourseStudents
                           .Include(cs => cs.Course)
                           .Include(cs => cs.Student)
                           .Where(cs => cs.Student.Name.ToLower().Contains(q) ||
                                        cs.Course.Name.ToLower().Contains(q))
                           .ToList();
        }

        public CourseStudents GetById(int id)
        {
            return _context.CourseStudents
                           .Include(cs => cs.Course)
                           .Include(cs => cs.Student)
                           .FirstOrDefault(cs => cs.Id == id);
        }

        public void Add(CourseStudents courseStudents)
        {
            _context.CourseStudents.Add(courseStudents);
            _context.SaveChanges();
        }

        public void Update(CourseStudents courseStudents)
        {
            _context.CourseStudents.Update(courseStudents);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var cs = _context.CourseStudents.Find(id);
            if (cs != null)
            {
                _context.CourseStudents.Remove(cs);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Course> GetCourses()
        {
            return _context.Courses.ToList();
        }

        public IEnumerable<Student> GetStudents()
        {
            return _context.Students.ToList();
        }
    }
}
