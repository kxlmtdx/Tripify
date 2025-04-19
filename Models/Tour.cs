using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class Tour
    {
        [Key]
        public int Tour_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [ForeignKey("Direction")]
        public int Direction_Id { get; set; }

        [ForeignKey("Hotel")]
        public int Hotel_Id { get; set; }

        [ForeignKey("TourType")]
        public int Tour_Type_Id { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        [StringLength(150)]
        public string? Image_Url { get; set; }
        [Required]
        public int Days { get; set; }

        public Direction Direction { get; set; }
        public Hotel Hotel { get; set; }
        public TourType TourType { get; set; }
    }
}
