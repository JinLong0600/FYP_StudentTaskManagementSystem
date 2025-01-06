using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L1StudentsViewModel
    {

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }
        public string Username { get; set; }
        
        public string EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public DateTime DOB { get; set; }

        public int Gender { get; set; }

        public string? State { get; set; }

        public string? GuardianRelationship { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianEmailAddress { get; set; }
        public string? GuardianContactNumber { get; set; }

        public string InstitutionName { get; set; }

        public IFormFile? StudentIdentityCard { get; set; }

        public int EducationStage { get; set; }

        public int EducationalYear { get; set; }

        public int? AccountStatus { get; set; }

        //[Required] public int DiscussionLevel { get; set; }

        public DateTime CreationDateTime { get; set; }

        public DateTime DeleteAccountDateTime { get; set; }

        public DateTime SuspensionDateTime { get; set; }

        public int VerifiedByAdminId { get; set; }

        // For displaying existing images
        public string? ExistingProfileImagePath { get; set; }
        public string? ExistingStudentIdentityCardPath { get; set; }

        //public bool isDailySummaryEnable { get; set; }

        //public DateTime DontDisturbStartTime { get; set; }

        //public DateTime DontDisturbEndTime { get; set; }

    }
}

