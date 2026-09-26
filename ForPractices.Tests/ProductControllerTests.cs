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


        [Fact]
        public async Task DeleteProduct_ReturnNotFound()
        {
            //arrange

            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);
            //act           

            var result = await productController.DeleteProduct(0);

            //Assert
            var NotFoundRequestResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product not found.", NotFoundRequestResult.Value);


            var productCount = await context.Products.CountAsync();
            Assert.Equal(0, productCount);
        }


        [Fact]
        public async Task DeleteProduct_ReturnOk()
        {
            //arrange
            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);

            ClaimsHelper.SetFakaUser(productController, 1);

            var createProduct = new ProductCreateDto
            {
                ProductName = "Tamim",
                ProductDescription = "new ara in market",
                ProductPrice = 100,
                ProductQuantity = 120
            };
            var prodcutCreate = await productController.CreateProduct(createProduct);

            var createResult = Assert.IsType<OkObjectResult>(prodcutCreate);
            var productCount = await context.Products.CountAsync();
            Assert.Equal(1, productCount);

            var result = await productController.DeleteProduct(1);

            //Assert
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Product deleted successfully.", okRequestResult.Value);


            productCount = await context.Products.CountAsync();
            Assert.Equal(0, productCount);

        }




        [Fact]
        public async Task UpdateProduct_ReturnBadRequest()
        {
            //arrange
            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();
            var IFormFileMock = new Mock<IFormFile>();


            var productController = new ProductController(context, fileUploadServiceMock.Object);


            var products = new ProductUpdateDto
            {
                ProductDescription = "new ara in market",
                ProductPrice = 100,
                ProductQuantity = 120
            };


            var result = await productController.UpdateProduct(1, products);

            //Assert
            var BadRequestRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product is empty.", BadRequestRequestResult.Value);

        }



        [Fact]
        public async Task UpdateProduct_ReturnNotFound()
        {
            //arrange
            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();
            var IFormFileMock = new Mock<IFormFile>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);


            var products = new ProductUpdateDto
            {
                ProductName = "Tometo",
                ProductDescription = "new ara in market",
                ProductPrice = 100,
                ProductQuantity = 120
            };


            var result = await productController.UpdateProduct(1, products);

            //Assert
            var NotFoundRequestResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Update user not found.", NotFoundRequestResult.Value);


            var userCount = await context.Products.CountAsync();
            Assert.Equal(0, userCount);

        }




        [Fact]
        public async Task UpdateProduct_ReturnOk()
        {
            //arrange
            var context = TestDbContextFactory.Create();

            var fileUploadServiceMock = new Mock<IFileUploadService>();
            var IFormFileMock = new Mock<IFormFile>();

            var productController = new ProductController(context, fileUploadServiceMock.Object);

            ClaimsHelper.SetFakaUser(productController, 1);

            //product create in product db
            var createProduct = new ProductCreateDto
            {
                ProductName = "Tamim",
                ProductDescription = "new ara in market",
                ProductPrice = 100,
                ProductQuantity = 120
            };
            var prodcutCreate = await productController.CreateProduct(createProduct);

            var createResult = Assert.IsType<OkObjectResult>(prodcutCreate);
            var productCount = await context.Products.CountAsync();
            Assert.Equal(1, productCount);


            var updateProduct = new ProductUpdateDto
            {
                ProductName = "Tometo",
                ProductDescription = "new ara in market aaaj jf f",
                ProductPrice = 11100,
                ProductQuantity = 15000
            };

            var result = await productController.UpdateProduct(1, updateProduct);

            //Assert
            var OkRequestResult = Assert.IsType<OkObjectResult>(result);


            var productFound = await context.Products.FirstOrDefaultAsync(x => x.ProductName == "Tamim");
            Assert.Null(productFound);

            var usercount = await context.Products.CountAsync();
            Assert.Equal(1, usercount);
        }

    }
}

