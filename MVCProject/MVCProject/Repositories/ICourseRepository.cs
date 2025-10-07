using MVCProject.Models;

namespace MVCProject.Repositories
{
    public interface ICourseRepository
    {
        IEnumerable<Course> GetAll();
        IEnumerable<Course> SearchByName(string searchString);
        Course GetById(int id);
        void Add(Course course);
        void Update(Course course);
        void Delete(int id);
        bool Exists(int id);
        IEnumerable<Department> GetDepartments();
    }
}
