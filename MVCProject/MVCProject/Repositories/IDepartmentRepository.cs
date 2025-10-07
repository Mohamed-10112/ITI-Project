using MVCProject.Models;

namespace MVCProject.Repositories
{
    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetAll();
        IEnumerable<Department> Search(string searchString);
        Department GetById(int id);
        void Add(Department department);
        void Update(Department department);
        void Delete(int id);
    }
}
