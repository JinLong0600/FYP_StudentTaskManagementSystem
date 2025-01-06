using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
