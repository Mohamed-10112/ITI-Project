using System.ComponentModel.DataAnnotations;

namespace MVCProject.ViewModels
{
    public class RoleViewModel
    {
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
