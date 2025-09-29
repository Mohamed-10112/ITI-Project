namespace MVCProject.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Degree { get; set; }
        public int MinimumDegree { get; set; }
        public int Hours { get; set; }

        public int DeptId { get; set; }
        public virtual Department Department { get; set; }

        public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
        public virtual ICollection<CourseStudents> CourseStudents { get; set; } = new List<CourseStudents>();
    }
}
