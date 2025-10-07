using MVCProject.Models;

namespace MVCProject.Repositories
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAll();
        IEnumerable<Student> Search(string searchString);
        Student GetById(int id);
        void Add(Student student);
        void Update(Student student);
        void Delete(int id);
        bool Exists(int id);
        IEnumerable<Department> GetDepartments();
    }
}
