using ForPractices.Controller;
using ForPractices.DTO.Pagination;
using ForPractices.DTO.Product;
using ForPractices.Model;
using ForPractices.Service.FileUpload;
using ForPractices.Tests.Helpers;
using Microsoft.AspNetCore.Http;
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

        [Fact]
        public async Task CreateProduct_ReturnsOK()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);

            ClaimsHelper.SetFakaUser(productController, 1);


            //act
            var createProduct = new ProductCreateDto
            {
                ProductName = "Tamim",
                ProductDescription = "new ara in market",
                ProductPrice = 100,
                ProductQuantity = 120,
                //Image = "data:image/jpeg;base64,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"
            };


            //call createProduct with empty creteProduct
            var result = await productController.CreateProduct(createProduct);

            //Assert
            var OKResult = Assert.IsType<OkObjectResult>(result);
            //Assert.Equal("Product name is required.", badRequestResult.Value);

            var productFound = await context.Products.FirstOrDefaultAsync(x => x.ProductName == "Tamim");
            Assert.NotNull(productFound);

            var productCount = await context.Products.CountAsync();
            Assert.Equal(1, productCount);
        }



        [Fact]
        public async Task CreateProduct_ReturnWithImage()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();
            var IFormFileMock = new Mock<IFormFile>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);

            ClaimsHelper.SetFakaUser(productController, 1);


            //act
            var createProduct = new ProductCreateDto
            {
                ProductName = "Tamim",
                ProductDescription = "new ara in market",
                ProductPrice = 100,
                ProductQuantity = 120,
                Image = IFormFileMock.Object,
            };

            fileUploadServiceMock.Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync("/uploads/products/fake-image.jpg");

            var result = await productController.CreateProduct(createProduct);

            //Assert
            var OKResult = Assert.IsType<OkObjectResult>(result);

            fileUploadServiceMock.Verify(x => x.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Once);

            //this name is find any product
            var productFound = await context.Products.FirstOrDefaultAsync(x => x.ProductName == "Tamim");
            Assert.NotNull(productFound);

            //setup verifing
            Assert.Equal("/uploads/products/fake-image.jpg", productFound.ImageUrl);

            //db product count
            var productCount = await context.Products.CountAsync();
            Assert.Equal(1, productCount);
        }
    }
}

