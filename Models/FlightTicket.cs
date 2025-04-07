using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TourFlow.Models
{
    public class FlightTicket
    {
        [Key]
        public int Ticket_Id { get; set; }

        [ForeignKey("Booking")]
        public int Booking_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Departure_City { get; set; }

        [Required]
        [StringLength(100)]
        public string Arrival_City { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Departure_Date { get; set; }  // Заменили DateOnly на DateTime

        [Required]
        public TimeSpan Departure_Time { get; set; }  // Заменили TimeOnly на TimeSpan

        [Required]
        public TimeSpan Arrival_Time { get; set; }    // Заменили TimeOnly на TimeSpan

        [ForeignKey("FlightType")]
        public int Flight_Type_Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [ForeignKey("Airline")]
        public int Airline_Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Flight_Number { get; set; }

        public Booking Booking { get; set; }
        public FlightType FlightType { get; set; }
        public Airline Airline { get; set; }

        // Добавляем вспомогательные свойства для удобства
        [NotMapped]
        public string FormattedDepartureTime => Departure_Time.ToString(@"hh\:mm");

        [NotMapped]
        public string FormattedArrivalTime => Arrival_Time.ToString(@"hh\:mm");

        [NotMapped]
        public string MonthName => new CultureInfo("ru-RU").DateTimeFormat.GetMonthName(Departure_Date.Month).ToLower();

        [NotMapped]
        public int Day => Departure_Date.Day;

        [NotMapped]
        public TimeSpan Duration => Arrival_Time - Departure_Time;
    }
}