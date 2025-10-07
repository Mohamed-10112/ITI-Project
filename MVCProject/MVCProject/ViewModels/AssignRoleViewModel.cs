using System.ComponentModel.DataAnnotations;

namespace MVCProject.ViewModels
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "User Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Selected Role")]
        public string SelectedRole { get; set; }

        public List<string> Roles { get; set; }
    }
}
