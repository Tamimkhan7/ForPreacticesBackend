using System.ComponentModel.DataAnnotations;

namespace ForPractices.DTO.Product
{
    public class ProductCreateDto
    {
        [Required]
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; } = 0;
        public decimal ProductQuantity { get; set; } = 0;
        public int UserId { get; set; }
    }
}
