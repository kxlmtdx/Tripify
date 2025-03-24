using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class AccountType
    {
        [Key]
        public int Account_Type_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }
    }
}
