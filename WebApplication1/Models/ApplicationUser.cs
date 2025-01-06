using Microsoft.AspNetCore.Identity;

namespace StudentTaskManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}
