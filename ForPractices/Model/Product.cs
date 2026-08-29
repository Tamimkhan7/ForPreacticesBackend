using System.ComponentModel.DataAnnotations;

namespace ForPractices.Model
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; } = 0;
        public decimal ProductQuantity { get; set; } = 0;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

    }
}

