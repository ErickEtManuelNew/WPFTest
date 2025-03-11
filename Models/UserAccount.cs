using System.ComponentModel.DataAnnotations;

namespace WPFTest.Models
{
    public enum UserRole
    {
        Viewer,
        Editor,
        Administrator        
    }

    public class UserAccount
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public required string PasswordHash { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [MaxLength(64)]
        public string? VerificationToken { get; set; }

        public DateTime? VerifiedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Publication> Publications { get; set; } = new List<Publication>();
    }
} 