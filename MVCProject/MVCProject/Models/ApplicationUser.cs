using Microsoft.AspNetCore.Identity;

namespace MVCProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DisplayName => $"{FirstName} {LastName}";

        public Student? StudentProfile { get; set; }
        public Instructor? InstructorProfile { get; set; }
    }
}
