using System.ComponentModel.DataAnnotations;

namespace TourFlow.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string? Type { get; set; }
    }
}