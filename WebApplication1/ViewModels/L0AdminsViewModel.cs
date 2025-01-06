using System.ComponentModel.DataAnnotations;

namespace StudentTaskManagement.ViewModels
{
    public class L0AdminsViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ProfileImage { get; set; }
        public DateTime DOB { get; set; }
        public int Address { get; set; }
        public int EmailAddress { get; set; }
        public int PhoneNumber { get; set; }
        public int CountryAccess { get; set; }
        public int CountryCode { get; set; }
        public int CityCode { get; set; }
        public int CreatedByAdminId { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public int LastModifiedByAdminId { get; set; }
        public DateTime DeletedDateTime { get; set; }
    }

}


