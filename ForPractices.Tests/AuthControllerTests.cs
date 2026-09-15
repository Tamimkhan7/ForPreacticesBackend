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

            //for check if an user already register use this email. 


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
    }

}

