using System.ComponentModel.DataAnnotations;

namespace MVCProject.ViewModels
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
