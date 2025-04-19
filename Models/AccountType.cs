using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourFlow.Models
{
    [Table("Accounts_Type")]
    public class AccountType
    {
        [Key]
        public int Account_Type_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }
    }
}
