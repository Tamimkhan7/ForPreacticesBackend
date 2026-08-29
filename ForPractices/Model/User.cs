using System.ComponentModel.DataAnnotations;

namespace ForPractices.Model
{
    public class User
    {
        public int Id { get; set; }
        [Required, StringLength(20)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = "User";
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
