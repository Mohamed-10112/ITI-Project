using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProject.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }

        public int DeptId { get; set; }
        public virtual Department Department { get; set; }

        public int CrsId { get; set; }
        public virtual Course Course { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; } 
        public virtual ApplicationUser User { get; set; }
    }
}
