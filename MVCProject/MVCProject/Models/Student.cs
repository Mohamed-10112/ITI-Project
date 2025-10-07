using System.ComponentModel.DataAnnotations;

namespace MVCProject.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        [Display(Name = "CGPA")]
        public double Grade { get; set; }

        public int DeptId { get; set; }
        public virtual Department Department { get; set; }

        public virtual ICollection<CourseStudents> CourseStudents { get; set; } = new List<CourseStudents>();

        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
