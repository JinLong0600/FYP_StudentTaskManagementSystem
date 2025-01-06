using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.Utilities
{
    public class GeneralEnum
    {
        public enum ForumCategory
        {
            GeneralDiscussions = 1,
            HomeworkHelp = 2,
            ExamsTestPrep = 3,
            InternshipsVolunteering = 4,
            ScholarshipsFinancialAid = 5,
            QnA = 6,
            StudyGroups = 7,
            PeerAdvice = 8,
        }
        public enum ForumStatus
        {
            Active = 1,
            Resolved = 2,
            Deleted = 3,
        }
        public enum PriorityLevel
        {
            Trivial = 1,
            Low = 2,
            Medium = 3,
            High = 4,
            Critical = 5,
        }

        public enum ItemTaskStatus
        {
            NotStarted = 1,     //Grey
            InProgress = 2,     //Blue
            Completed = 3,      //Green
            OnHold = 4,         //Yellow
            Overdue = 5,     //Orange
        }
        public enum ItemTaskCategory
        {
            Academic = 1,
            Extracurricular = 2,
            PersonalDevelopment = 3,
            Social = 4,
            HealthWellness = 5,
            Miscellaneous = 6
        }

        public enum RecurringType
        {
            Day = 1,
            Week = 2,
            BiWeek = 3,
            Month = 4,
            BiMonth = 5,
        }

        public enum RecurringBi
        {
            Odd = 1,
            Even = 2,
        }

        public enum DefaultRecurringOptions
        {
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
        }



        public enum DefaultNotificationOptions
        {
            ThirtyMin = 1,
            OneWeek = 2,
            OneMonth = 3,
        }

        public enum EducationStage
        { 
            Primary = 1,
            Secondary = 2, 
            HighEducation = 3,
        }

        public enum GuardianRelationship
        {
            Parent = 1,
            Grandparent = 2,
            Guardian = 3,
        }

        public enum StudentAccountStatus
        {
            Deactivated = 1,
            Suspended = 2,
            Active = 3,
/*          Verified,
            PendingVerification,
            Unverified*/
        }

        public enum Gender
        {
            Male = 1,
            Female = 2,
        }

        public enum DeleteBehavior
        { 
            ClientSetNull,
            Restrict,
            SetNull,
            Cascade
        }

        public enum NotificationPresetType
        { 
            Days = 1,
            Mintues = 2
        }

        public enum PresetStatus
        {
            Active = 1,
            Deleted = 0
        }
    }
}
