using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class BookingStatusType
    {
        [Key]
        public int Booking_Status_Type_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Booking_Type { get; set; }
    }
}
