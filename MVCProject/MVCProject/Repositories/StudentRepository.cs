using MVCProject.Models;
using Microsoft.EntityFrameworkCore;

namespace MVCProject.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Student> GetAll()
        {
            return _context.Students.Include(s => s.Department).ToList();
        }

        public IEnumerable<Student> Search(string searchString)
        {
            var q = searchString.Trim().ToLower();
            return _context.Students
                .Include(s => s.Department)
                .Where(s => s.Name.ToLower().Contains(q) ||
                            (s.Department != null && s.Department.Name.ToLower() == q))
                .ToList();
        }

        public Student GetById(int id)
        {
            return _context.Students.Include(s => s.Department).Include(s => s.User).FirstOrDefault(s => s.Id == id);
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id)
        {
            return _context.Students.Any(s => s.Id == id);
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _context.Departments.ToList();
        }
    }
}
