using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1Students : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [StringLength(500)]
        public string? ProfileImage { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public int Gender { get; set; }

        public string? GuardianRelationship { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianEmailAddress { get; set; }
        public string? GuardianContactNumber { get; set; }

        [Required(AllowEmptyStrings =true)]
        public string InstitutionName { get; set; }

        public string? StudentIdentityCard { get; set; }

        [Required]
        public int EducationStage { get; set; }

        [Required]
        public int EducationalYear { get; set; }

        [Required]
        public int AccountStatus { get; set; }

        //[Required] public int DiscussionLevel { get; set; }

        [Required]
        public DateTime CreationDateTime { get; set; }

        public DateTime DeleteAccountDateTime { get; set; }

        public DateTime SuspensionDateTime { get; set; }

        public int VerifiedByAdminId { get; set; }

        //public bool isDailySummaryEnable { get; set; }

        //public DateTime DontDisturbStartTime { get; set; }

        //public DateTime DontDisturbEndTime { get; set; }
    }
}

