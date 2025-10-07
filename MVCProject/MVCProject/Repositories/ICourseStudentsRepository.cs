using MVCProject.Models;

namespace MVCProject.Repositories
{
    public interface ICourseStudentsRepository
    {
        IEnumerable<CourseStudents> GetAll();
        IEnumerable<CourseStudents> Search(string searchString);
        CourseStudents GetById(int id);
        void Add(CourseStudents courseStudents);
        void Update(CourseStudents courseStudents);
        void Delete(int id);
        IEnumerable<Course> GetCourses();   
        IEnumerable<Student> GetStudents();
    }
}
