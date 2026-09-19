using ForPractices.Controller;
using ForPractices.DTO.Pagination;
using ForPractices.DTO.Product;
using ForPractices.Model;
using ForPractices.Service.FileUpload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ForPractices.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task GetProduct_ReturnsAllProducts()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            context.Products.AddRange(
                new Product { ProductName = "Laptop", ProductPrice = 50000, ProductQuantity = 5 },
                new Product { ProductName = "Mouse", ProductPrice = 500, ProductQuantity = 20 },
                new Product { ProductName = "Keyboard", ProductPrice = 2500, ProductQuantity = 15 },
                new Product { ProductName = "Monitor", ProductPrice = 5523, ProductQuantity = 10 }
                );

            await context.SaveChangesAsync();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);


            //act

            //pagination default value is PageNumber = 1, PageSize = 12
            var pagination = new PaginationParams();
            //call getProduct with pagination
            var result = await productController.GetProduct(pagination);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsType<PagedResult<Product>>(okResult.Value);


            Assert.Equal(4, pagedResult.TotalCount);
            Assert.Equal(4, pagedResult.Items.Count);
            Assert.Equal(1, pagedResult.PageNumber);
        }



        [Fact]
        public async Task GetProduct_ReturnsBySearchProducts()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            context.Products.AddRange(
                new Product { ProductName = "Laptop", ProductPrice = 50000, ProductQuantity = 5 },
                new Product { ProductName = "Mouse", ProductPrice = 500, ProductQuantity = 20 },
                new Product { ProductName = "Keyboard", ProductPrice = 2500, ProductQuantity = 15 },
                new Product { ProductName = "Monitor", ProductPrice = 5523, ProductQuantity = 10 }
                );

            await context.SaveChangesAsync();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);


            //act

            //pagination default value is PageNumber = 1, PageSize = 12
            var pagination = new PaginationParams
            {
                SearchValue = "LaP"
            };
            //call getProduct with pagination
            var result = await productController.GetProduct(pagination);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsType<PagedResult<Product>>(okResult.Value);


            Assert.Equal(1, pagedResult.TotalCount);
            Assert.Equal("Laptop", pagedResult.Items[0].ProductName);
            Assert.Equal(1, pagedResult.PageNumber);
        }

        [Fact]
        public async Task GetProduct_ReturnsByMinProducts()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            context.Products.AddRange(
                new Product { ProductName = "Laptop", ProductPrice = 50000, ProductQuantity = 5 },
                new Product { ProductName = "Mouse", ProductPrice = 500, ProductQuantity = 20 },
                new Product { ProductName = "Keyboard", ProductPrice = 2500, ProductQuantity = 15 },
                new Product { ProductName = "Monitor", ProductPrice = 5523, ProductQuantity = 10 }
                );

            await context.SaveChangesAsync();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);


            //act

            //pagination default value is PageNumber = 1, PageSize = 12
            var pagination = new PaginationParams
            {
                minValue = 600
            };
            //call getProduct with pagination
            var result = await productController.GetProduct(pagination);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsType<PagedResult<Product>>(okResult.Value);


            Assert.Equal(3, pagedResult.TotalCount);
            Assert.Equal(3, pagedResult.Items.Count);
            Assert.Equal(1, pagedResult.PageNumber);
        }

        [Fact]
        public async Task GetProduct_ReturnsMinBetweenMaxProducts()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            context.Products.AddRange(
                new Product { ProductName = "Laptop", ProductPrice = 50000, ProductQuantity = 5 },
                new Product { ProductName = "Mouse", ProductPrice = 500, ProductQuantity = 20 },
                new Product { ProductName = "Keyboard", ProductPrice = 2500, ProductQuantity = 15 },
                new Product { ProductName = "Monitor", ProductPrice = 5523, ProductQuantity = 10 }
                );

            await context.SaveChangesAsync();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);


            //act

            //pagination default value is PageNumber = 1, PageSize = 12
            var pagination = new PaginationParams
            {
                minValue = 500,
                maxValue = 30000
            };
            //call getProduct with pagination
            var result = await productController.GetProduct(pagination);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsType<PagedResult<Product>>(okResult.Value);


            Assert.Equal(3, pagedResult.TotalCount);
            Assert.Equal(3, pagedResult.Items.Count);
            Assert.Equal(1, pagedResult.PageNumber);
        }

        [Fact]
        public async Task GetProduct_ReturnsFullSearchProducts()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            context.Products.AddRange(
                new Product { ProductName = "Laptop", ProductPrice = 50000, ProductQuantity = 5 },
                new Product { ProductName = "Mouse", ProductPrice = 500, ProductQuantity = 20 },
                new Product { ProductName = "Keyboard", ProductPrice = 2500, ProductQuantity = 15 },
                new Product { ProductName = "Monitor", ProductPrice = 5523, ProductQuantity = 10 }
                );

            await context.SaveChangesAsync();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);


            //act

            //pagination default value is PageNumber = 1, PageSize = 12
            var pagination = new PaginationParams
            {
                SearchValue = "moU",
                minValue = 500,
                maxValue = 30000
            };
            //call getProduct with pagination
            var result = await productController.GetProduct(pagination);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsType<PagedResult<Product>>(okResult.Value);


            Assert.Equal(1, pagedResult.TotalCount);
            Assert.Equal(1, pagedResult.Items.Count);
            Assert.Equal(1, pagedResult.PageNumber);
        }



        [Fact]
        public async Task GetProduct_ReturnsEmptyProducts()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);


            //act

            //pagination default value is PageNumber = 1, PageSize = 12
            var pagination = new PaginationParams();

            //call getProduct with pagination
            var result = await productController.GetProduct(pagination);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsType<PagedResult<Product>>(okResult.Value);


            Assert.Equal(0, pagedResult.TotalCount);
            Assert.Empty(pagedResult.Items);
            Assert.Equal(1, pagedResult.PageNumber);

        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequestWithEmptyModel()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);


            //act
            var createProduct = new ProductCreateDto();


            //call createProduct with empty creteProduct
            var result = await productController.CreateProduct(createProduct);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product name is required.", badRequestResult.Value);


            var productCount = await context.Products.CountAsync();
            Assert.Equal(0, productCount);
        }
    }
}

