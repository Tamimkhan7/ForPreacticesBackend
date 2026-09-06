using ForPractices.Model;
using Microsoft.EntityFrameworkCore;

namespace ForPractices.Data
{
    public static class DbSeeder
    {
        public static async Task Seed(AppDbContext context)
        {
            var user = await context.Users.AnyAsync(x => x.Role == "Admin");

            if (!user)
            {
                var admin = new User
                {
                    Name = "Admin",
                    Email = "admin123@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin"
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }

        }

    }
}
