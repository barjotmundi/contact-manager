using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models
{
    public class Contact
    {
        public Guid Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        [RegularExpression(@"\(\d{3}\)-\d{3}-\d{4}", ErrorMessage = "Phone must be (123)-456-7890")]
        public required string Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
