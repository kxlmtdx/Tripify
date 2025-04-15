using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TourFlow.Models
{
    public class User_Document
    {
        [Key]
        public int Document_Id { get; set; }

        [ForeignKey("Account")]
        public int Account_Id { get; set; }

        [ForeignKey("DocumentType")]
        public int Document_Type_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Last_Name { get; set; }

        [Required]
        [StringLength(50)]
        public string First_Name { get; set; }

        [StringLength(50)]
        public string Middle_Name { get; set; }

        public DateTime Date_Of_Birth { get; set; }

        public int Document_Number { get; set; }

        [StringLength(50)]
        public string? International_Last_Name { get; set; }

        [StringLength(50)]
        public string? International_First_Name { get; set; }

        public int? International_Document_Number { get; set; }

        public DateTime? Expiration_Date { get; set; }

        public Account Account { get; set; }
        public DocumentType Document_Type { get; set; }
    }
}
