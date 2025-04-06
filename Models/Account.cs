using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace TourFlow.Models
{
    public class Account
    {
        [Key]
        public int Account_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [ForeignKey("AccountType")]
        public int Account_Type_Id { get; set; }

        [StringLength(50)]
        public string? fName { get; set; }

        public AccountType AccountType { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<User_Document> User_Documents { get; set; } = new List<User_Document>();
    }
}
