using ForPractices.Controller;
using ForPractices.DTO.Auth;
using ForPractices.Model;
using ForPractices.Service.Email;
using ForPractices.Service.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ForPractices.Tests
{
    public class AuthControllerTests
    {


        [Fact]
        public async Task Register_WithNewEmail_ReturnsOk()
        {
            //Arrange
            var context = TestDbContextFactory.Create();

            var configValues = new Dictionary<string, string>
                {
                    {"Jwt:key", "ThisIsASecretKeyForTestingPurposeOnly123!" },
                    {"Jwt:Issuer", "TestIssuer" },
                    {"Jwt:Audience", "TestAudience" },
                    {"Jwt:AccessTokenExpiryMinutes", "15" }
                };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            var jwtTokenService = new JwtTokenService(config);
            var emailServiceMock = new Mock<IEmailService>();

            var authController = new AuthController(context, config, jwtTokenService, emailServiceMock.Object);

            var newUser = new RegisterRequestDto
            {
                Name = "Test User",
                Email = "newuser@test.com",
                Password = "TestPassword123!"
            };

            var result = await authController.Register(newUser);

            //assert
            //this part check is it return a object is correct or wrong
            var OkResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully Registered", OkResult.Value);

            //act

            var savedUser = await context.Users.FirstOrDefaultAsync(x => x.Email == "newuser@test.com");
            Assert.NotNull(savedUser);
            Assert.Equal("Test User", savedUser.Name);

        }


        [Fact]
        public async Task Register_WithExistingEmail_ReturnsBadRequest()
        {
            //Arrange
            var context = TestDbContextFactory.Create();

            var configValues = new Dictionary<string, string>
            {
                {"Jwt:key", "ThisIsASecretKeyForTestingPurposeOnly123!" },
                {"Jwt:Issuer", "TestIssuer" },
                {"Jwt:Audience", "TestAudience" },
                {"Jwt:AccessTokenExpiryMinutes", "15" }
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            var jwtTokenService = new JwtTokenService(config);
            var emailServiceMock = new Mock<IEmailService>();

            //already seed the db, one user info store in the db

            var existingUser = new User
            {
                Name = "Existing User",
                Email = "existing@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("somePassword123!")
            };

            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var authController = new AuthController(context, config, jwtTokenService, emailServiceMock.Object);

            var duplicateUser = new RegisterRequestDto
            {
                Name = "Another Name",
                Email = "existing@test.com",
                Password = "AnotherPassword123!"
            };

            var result = await authController.Register(duplicateUser);

            //assert
            //this part check is it return a object is correct or wrong
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Already register at this email", badRequestResult.Value);

            var userCount = await context.Users.CountAsync();
            Assert.Equal(1, userCount);
        }

        [Fact]
        public async Task Register_WithEmptyName_ReturnsOk()
        {
            //Arrange
            var context = TestDbContextFactory.Create();

            var configValues = new Dictionary<string, string>
            {
                {"Jwt:key", "ThisIsASecretKeyForTestingPurposeOnly123!" },
                {"Jwt:Issuer", "TestIssuer" },
                {"Jwt:Audience", "TestAudience" },
                {"Jwt:AccessTokenExpiryMinutes", "15" }
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            var jwtTokenService = new JwtTokenService(config);
            var emailServiceMock = new Mock<IEmailService>();

            var authController = new AuthController(context, config, jwtTokenService, emailServiceMock.Object);

            //for checking purpose i could be name empty

            var newUser = new RegisterRequestDto
            {
                Name = "",
                Email = "emptyname@test.com",
                Password = "AnotherPassword123!"
            };

            var result = await authController.Register(newUser);

            //assert
            //this part check is it return a object is correct or wrong
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully Registered", okResult.Value);

            var savedUser = await context.Users.FirstOrDefaultAsync(x => x.Email == "emptyname@test.com");
            Assert.NotNull(savedUser);
            Assert.Equal("", savedUser.Name);
        }


        [Fact]
        public async Task Register_WithEmptyPassword_ReturnsOk()
        {
            // Arrange
            var context = TestDbContextFactory.Create();

            var configValues = new Dictionary<string, string>
    {
        { "Jwt:key", "ThisIsASecretKeyForTestingPurposeOnly123!" },
        { "Jwt:Issuer", "TestIssuer" },
        { "Jwt:Audience", "TestAudience" },
        { "Jwt:AccessTokenExpiryMinutes", "15" }
    };
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            var jwtTokenService = new JwtTokenService(config);
            var emailServiceMock = new Mock<IEmailService>();

            var authController = new AuthController(context, config, jwtTokenService, emailServiceMock.Object);

            var newUser = new RegisterRequestDto
            {
                Name = "Test User",
                Email = "emptypassword@test.com",
                Password = ""
            };

            // Act
            var result = await authController.Register(newUser);


            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully Registered", okResult.Value);

            var savedUser = await context.Users.FirstOrDefaultAsync(x => x.Email == "emptypassword@test.com");
            Assert.NotNull(savedUser);
            Assert.False(string.IsNullOrEmpty(savedUser.PasswordHash));
        }
    }
}

