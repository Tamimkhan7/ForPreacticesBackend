using ForPractices.Data;
using ForPractices.DTO.Pagination;
using ForPractices.DTO.Product;
using ForPractices.Extensions;
using ForPractices.Model;
using ForPractices.Service.FileUpload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ForPractices.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public ProductController(AppDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct([FromQuery] PaginationParams pagination)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.SearchValue))
                query = query.Where(p => p.ProductName.ToLower().Trim().Contains(pagination.SearchValue.ToLower().Trim()));

            if (pagination.minValue.HasValue)
                query = query.Where(p => p.ProductPrice >= pagination.minValue.Value);

            if (pagination.maxValue.HasValue)
                query = query.Where(p => p.ProductPrice <= pagination.maxValue.Value);


            var product = await query
              .OrderByDescending(p => p.CreateAt)
              .ToPagedResultAsync(pagination);

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto create)
        {
            if (string.IsNullOrWhiteSpace(create.ProductName))
                return BadRequest("Product name is required.");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            string? imageUrl = null;
            if (create.Image != null)
                imageUrl = await _fileUploadService.UploadFileAsync(create.Image, "ProductImages");


            var product = new Product
            {
                ProductName = create.ProductName,
                ProductDescription = create.ProductDescription,
                ProductPrice = create.ProductPrice,
                ProductQuantity = create.ProductQuantity,
                CreateAt = DateTime.UtcNow,
                UserId = userId,
                ImageUrl = imageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateProduct(int Id, [FromForm] ProductUpdateDto update)
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

            if (update.Image != null)
            {
                _fileUploadService.DeleteFile(Exists.ImageUrl);
                Exists.ImageUrl = await _fileUploadService.UploadFileAsync(update.Image, "ProductImages");
            }

            await _context.SaveChangesAsync();

            return Ok(Exists);


        }

        [Authorize(Roles = "Admin")]
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