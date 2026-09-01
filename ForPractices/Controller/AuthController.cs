using ForPractices.Data;
using ForPractices.DTO.Auth;
using ForPractices.Model;
using ForPractices.Service.Email;
using ForPractices.Service.JWT;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IEmailService _emailService;

        public AuthController(AppDbContext context, IConfiguration config, JwtTokenService jwtTokenService, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto register)
        {
            register.Email = register.Email.Trim().ToString();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == register.Email);
            if (user != null)
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

            login.Email = login.Email.Trim().ToString();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == login.Email);
            if (user == null)
                return BadRequest("Invalid email or password");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password");

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            var refreshDays = double.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshDays);

            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogOut(RefreshTokenDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Logged out successfully");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            request.Email = request.Email.Trim().ToString();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
                return Ok("If this email user, a reset link has been sent");

            var resetToken = _jwtTokenService.GenerateRefreshToken();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            var body = $"<h3>Password Reset</h3><p>Your reset token:</p><p><b>{resetToken}</b></p><p>This expires in 15 minutes.</p>";
            await _emailService.SendEmailAsync(user.Email, "Reset your password", body);

            return Ok("If this email user, a reset link has been sent");
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();


            var refreshDays = double.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshDays);

            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }


    }
}
