using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class Hotel
    {
        [Key]
        public int Hotel_Id { get; set; }

        [ForeignKey("Direction")]
        public int Direction_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(1, 5)]
        public int Stars { get; set; }

        [StringLength(3)]
        public string Nutrition { get; set; }

        public Direction Direction { get; set; }
    }
}
