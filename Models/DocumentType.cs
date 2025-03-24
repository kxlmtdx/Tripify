using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class DocumentType
    {
        [Key]
        public int Document_Type_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Document_Type { get; set; }
    }
}
