using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTaskManagement.Models
{
    public class L1Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public int CreatedByStudentId { get; set; }

        [Required(AllowEmptyStrings = true)]
        public DateTime LastModifiedDateTime { get; set; }

        [Required(AllowEmptyStrings = true)]
        public DateTime DeletionDateTime { get; set; }

    }

}
