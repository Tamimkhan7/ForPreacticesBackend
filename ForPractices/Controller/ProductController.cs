using ForPractices.Data;
using ForPractices.DTO.Product;
using ForPractices.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ForPractices.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            if (!_context.Products.Any())
                return NotFound("No Products found.");

            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto create)
        {
            if (string.IsNullOrWhiteSpace(create.ProductName))
                return BadRequest("Product name is required.");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var product = new Product
            {
                ProductName = create.ProductName,
                ProductDescription = create.ProductDescription,
                ProductPrice = create.ProductPrice,
                ProductQuantity = create.ProductQuantity,
                CreateAt = DateTime.UtcNow,
                UserId = create.UserId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateProduct(int Id, ProductUpdateDto update)
        {
            if (string.IsNullOrWhiteSpace(update.ProductName))
                return BadRequest("Product is empty.");

            var Exists = await _context.Products.FindAsync(Id);
            if (Exists == null)
                return NotFound("Update user not found.");

            Exists.ProductName = update.ProductName;
            Exists.ProductDescription = update.ProductDescription;
            Exists.ProductPrice = update.ProductPrice;
            Exists.ProductQuantity = update.ProductQuantity;
            Exists.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(Exists);


        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Product deleted successfully.");
        }

    }
}