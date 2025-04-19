using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourFlow.Models
{
    [Table("Booking_Status_Type")]
    public class BookingStatusType
    {
        [Key]
        public int Booking_Status_Type_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Booking_Type { get; set; }
    }
}
