using ForPractices.Model;
using Microsoft.EntityFrameworkCore;

namespace ForPractices.Data
{
    public static class DbSeeder
    {
        public static async Task Seed(AppDbContext context, IConfiguration config)
        {
            if (await context.Users.AnyAsync(x => x.Role == "Admin")) return;

            var name = config["Admin:Name"] ?? "Admin";
            var email = config["Admin:Email"];
            var password = config["Admin:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || password.Length < 12)
                return;

            var admin = new User
            {
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin"
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();


        }

    }
}
