using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class TourType
    {
        [Key]
        public int Tour_Type_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }
    }
}
