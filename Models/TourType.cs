using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourFlow.Models
{
    [Table("Tours_Type")]
    public class TourType
    {
        [Key]
        public int Tour_Type_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }
    }
}
