using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using StudentTaskManagement.Utilities;
using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class RegistrationViewModel
    {
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller:"Account")]
        [ValidEmailDomain(allowedDomain: "gmail.com", ErrorMessage = "Email domain must be gmail.com")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare ("Password", ErrorMessage = "Password is diffe")]
        public string ConfirmPassword { get; set; }



    }
}
