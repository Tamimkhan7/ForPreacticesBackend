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

        [Fact]
        public async Task Login_ReturnsBadRequest()
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

            var loginTest = new LoginRequestDto
            {
                Email = "tamim@test1.com",
                Password = "somePassword1231"
            };

            // Act
            var result = await authController.Login(loginTest);


            var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid email or password", BadRequestResult.Value);
        }


        [Fact]
        public async Task Login_ReturnsUnauthorized()
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

            var User = new RegisterRequestDto
            {
                Name = "Tamim",
                Email = "tamim@test.com",
                Password = "somePassword123",
            };

            var registerEmail = await authController.Register(User);
            var userfound = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(userfound);


            var loginTest = new LoginRequestDto
            {
                Email = "tamim@test.com",
                Password = "somePassword1231"
            };

            // Act
            var result = await authController.Login(loginTest);


            var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid email or password", UnauthorizedResult.Value);

            var savedUser = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(savedUser);
            Assert.False(string.IsNullOrEmpty(savedUser.PasswordHash));
        }

        [Fact]
        public async Task Login_ReturnsOk()
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

            var User = new RegisterRequestDto
            {
                Name = "Tamim",
                Email = "tamim@test.com",
                Password = "somePassword123!"
            };

            var registerEmail = await authController.Register(User);
            var userfound = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(userfound);


            var loginTest = new LoginRequestDto
            {
                Email = "tamim@test.com",
                Password = "somePassword123!"
            };

            // Act
            var result = await authController.Login(loginTest);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var authResponse = Assert.IsType<AuthResponseDto>(okResult.Value);


            Assert.False(string.IsNullOrEmpty(authResponse.AccessToken));
            Assert.False(string.IsNullOrEmpty(authResponse.RefreshToken));


            var savedUser = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(savedUser);
            Assert.False(string.IsNullOrEmpty(savedUser.PasswordHash));
            Assert.False(string.IsNullOrEmpty(savedUser.RefreshToken));
        }


        [Fact]
        public async Task ForgotPassword_ReturnsOk()
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

            var ForgotPasswordTest = new ForgotPasswordDto
            {
                Email = "nobody@test.com"
            };

            var result = await authController.ForgotPassword(ForgotPasswordTest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("If this email user, a reset link has been sent", okResult.Value);

            emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        [Fact]
        public async Task ForgotPassword_ReturnsFinallySentResetlink()
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

            var User = new RegisterRequestDto
            {
                Name = "Tamim",
                Email = "tamim@test.com",
                Password = "somePassword123!"
            };

            var registerEmail = await authController.Register(User);

            var userfound = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(userfound);

            var ForgotPasswordTest = new ForgotPasswordDto
            {
                Email = "tamim@test.com"
            };

            var result = await authController.ForgotPassword(ForgotPasswordTest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("If this email user, a reset link has been sent", okResult.Value);


            var userCheck = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(userCheck);
            Assert.False(string.IsNullOrEmpty(userCheck.PasswordResetToken));

            emailServiceMock.Verify(x => x.SendEmailAsync(It.Is<string>(b => b.Contains("tamim@test.com")), It.Is<string>(b => b.Contains("Reset your password")), It.Is<string>(b => b.Contains(userCheck.PasswordResetToken))), Times.Once);


            Assert.True((userCheck.PasswordResetTokenExpiry < DateTime.UtcNow.AddMinutes(16)) && (userCheck.PasswordResetTokenExpiry > DateTime.UtcNow));
        }


        [Fact]
        public async Task ResetPassword_WithUnknownToken_ReturnsBadRequest()
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

            var User = new RegisterRequestDto
            {
                Name = "Tamim",
                Email = "tamim@test.com",
                Password = "somePassword123!"
            };

            var mainPassword = User.Password;

            var registerEmail = await authController.Register(User);

            var userfound = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(userfound);


            var resetPasswordTest = new ResetPasswordDto
            {
                Token = "abced1654545615564654654654",
                NewPassword = "newpassword123!"
            };

            var result = await authController.ResetPassword(resetPasswordTest);

            var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid or expired reset token", BadRequestResult.Value);


            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(user);

            Assert.True(BCrypt.Net.BCrypt.Verify(mainPassword, user.PasswordHash));
            Assert.Null(user.PasswordResetToken);
        }

        [Fact]
        public async Task ResetPassword_WithExpiryToken_ReturnsBadRequest()
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

            context.Users.Add(new User
            {
                Name = "Tamim",
                Email = "tamim@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!"),
                PasswordResetToken = "abced1654545615564654654654",
                PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(-5)
            });
            await context.SaveChangesAsync();

            var oldPassword = "oldPassword123!";

            var resetPasswordTest = new ResetPasswordDto
            {
                Token = "abced1654545615564654654654",
                NewPassword = "newpassword123!"
            };

            var result = await authController.ResetPassword(resetPasswordTest);

            var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid or expired reset token", BadRequestResult.Value);


            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(user);

            Assert.True(BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash));
            Assert.True(user.PasswordResetTokenExpiry < DateTime.UtcNow);
        }

        [Fact]
        public async Task ResetPassword_WithValidToken_ReturnsOk()
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

            context.Users.Add(new User
            {
                Name = "Tamim",
                Email = "tamim@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!"),
                PasswordResetToken = "abced1654545615564654654654",
                PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10)
            });
            await context.SaveChangesAsync();

            var oldPassword = "oldPassword123!";

            var resetPasswordTest = new ResetPasswordDto
            {
                Token = "abced1654545615564654654654",
                NewPassword = "newpassword123!"
            };

            var result = await authController.ResetPassword(resetPasswordTest);

            var OkResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("password has been reset successfully", OkResult.Value);


            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(user);

            Assert.False(BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash));
            Assert.Null(user.PasswordResetToken);
            Assert.Null(user.PasswordResetTokenExpiry);
        }




        [Fact]
        public async Task ResetPassword_WithMissingToken_ReturnsBadRequest()
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

            context.Users.Add(new User
            {
                Name = "Tamim",
                Email = "tamim@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!")
            });
            await context.SaveChangesAsync();

            var oldPassword = "oldPassword123!";

            var resetPasswordTest = new ResetPasswordDto
            {
                Token = null!,
                NewPassword = "newpassword123!"
            };

            var result = await authController.ResetPassword(resetPasswordTest);

            var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid or expired reset token", BadRequestResult.Value);


            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == "tamim@test.com");
            Assert.NotNull(user);

            Assert.True(BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash));
            Assert.False(BCrypt.Net.BCrypt.Verify("newpassword123!", user.PasswordHash));
            Assert.Null(user.PasswordResetToken);
            Assert.Null(user.PasswordResetTokenExpiry);
        }

    }
}

