using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L0Admins
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(255)]
        [Required]
        public string LastName { get; set; }

        [StringLength(500)]
        public int ProfileImage { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [StringLength(255)]
        [Required]
        public int Address { get; set; }

        [StringLength(255)]
        [Required]
        public int EmailAddress { get; set; }
        
        [StringLength(50)]
        [Required]
        public int PhoneNumber { get; set; }

        public int CountryAccess { get; set; }
        
        [StringLength(2)]
        [Required]
        public int CountryCode { get; set; }

        [StringLength(3)]
        [Required]
        public int CityCode { get; set; }

        [Required]
        public int CreatedByAdminId { get; set; }

        [Required]
        public DateTime LastModifiedDateTime { get; set; }
        
        [Required]
        public int LastModifiedByAdminId { get; set; }

        [Required]
        public DateTime DeletedDateTime { get; set; }
    }

}


