using ForPractices.Controller;
using ForPractices.DTO.Auth;
using ForPractices.Service.Email;
using ForPractices.Service.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ForPractices.Tests
{
    public class AuthControllerTests
    {
        //Arrange

        [Fact]
        public async Task Register_WithNewEmail_ReturnsOk()
        {

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

            //Act

            var newUser = new RegisterRequestDto
            {
                Name = "Test User",
                Email = "newuser@test.com",
                Password = "TestPassword123!"
            };

            var result = await authController.Register(newUser);

            //assert
            //this part check is it return object is correct
            var OkResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully registered", OkResult.Value);



        }

    }
}
