using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class FlightType
    {
        [Key]
        public int Flight_Type_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Type_Name { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }
    }
}
