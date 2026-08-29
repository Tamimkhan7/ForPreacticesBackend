using ForPractices.Data;
using ForPractices.DTO.Auth;
using ForPractices.Model;
using ForPractices.Service.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForPractices.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(AppDbContext context, IConfiguration config, JwtTokenService jwtTokenService)
        {
            _context = context;
            _config = config;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto register)
        {

            var exists = await _context.Users.FirstOrDefaultAsync(x => x.Email == register.Email);
            if (exists != null)
                return BadRequest("Already register at this email");

            var users = new User
            {
                Name = register.Name,
                Email = register.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.Password)
            };

            _context.Users.Add(users);
            await _context.SaveChangesAsync();
            return Ok("Successfully Registered");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto login)
        {

            var exists = await _context.Users.FirstOrDefaultAsync(x => x.Email == login.Email);
            if (exists == null)
                return BadRequest("Invalid email or password");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, exists.PasswordHash))
                return Unauthorized("Invalid email or password");

            var accessToken = _jwtTokenService.GenerateAccessToken(exists);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            var refreshDays = double.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");
            exists.RefreshToken = refreshToken;
            exists.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshDays);

            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
                return Ok("If this email exists, a reset link has been sent");

            var resetToken = _jwtTokenService.GenerateRefreshToken();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Reset token generated",
                resetToken,
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PasswordResetToken == request.Token);

            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return BadRequest("Invalid or expired reset token");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("password has been reset successfully");
        }
    }
}
