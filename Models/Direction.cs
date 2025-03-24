using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class Direction
    {
        [Key]
        public int Direction_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }
    }
}
