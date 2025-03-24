using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class Airline
    {
        [Key]
        public int Airline_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Airline_Name { get; set; }

        [Required]
        [StringLength(3)]
        public string IATA_Code { get; set; }
    }
}
