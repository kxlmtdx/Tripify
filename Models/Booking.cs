using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class Booking
    {
        [Key]
        public int Booking_Id { get; set; }

        [ForeignKey("Account")]
        public int Account_Id { get; set; }

        [ForeignKey("Tour")]
        public int? Tour_Id { get; set; }

        [ForeignKey("Airline")]
        public int Airline_Id { get; set; }

        public DateTime Booking_Date { get; set; } = DateTime.Now;

        [ForeignKey("BookingStatusType")]
        public int Booking_Status_Id { get; set; }

        [ForeignKey("FlightTicket")]
        public int? Flight_Ticket_Id { get; set; }

        [ForeignKey("Hotel")]
        public int? Hotel_Id { get; set; }

        public Account Account { get; set; }
        public Tour Tour { get; set; }
        public Airline Airline { get; set; }
        public BookingStatusType BookingStatusType { get; set; }
        public Hotel Hotel { get; set; }
        public List<FlightTicket> FlightTickets { get; set; } = new List<FlightTicket>();
    }
}
