using MVCProject.Models;

namespace MVCProject.Repositories
{
    public interface IInstructorRepository
    {
        IEnumerable<Instructor> GetAll();
        IEnumerable<Instructor> Search(string searchString);
        Instructor GetById(int id);
        void Add(Instructor instructor);
        void Update(Instructor instructor);
        void Delete(int id);
        IEnumerable<Course> GetCourses();
        IEnumerable<Department> GetDepartments();
    }
}
