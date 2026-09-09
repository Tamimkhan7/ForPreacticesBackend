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
        public IFormFile? Image { get; set; } //receiving image file from the client, and image hold:- fileName, filePath, fileType, fileSize, data received from the client from-data.
    }
}
