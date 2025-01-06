using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
