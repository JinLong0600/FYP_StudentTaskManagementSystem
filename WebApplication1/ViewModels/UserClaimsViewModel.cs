using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class UserClaims
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }
}
