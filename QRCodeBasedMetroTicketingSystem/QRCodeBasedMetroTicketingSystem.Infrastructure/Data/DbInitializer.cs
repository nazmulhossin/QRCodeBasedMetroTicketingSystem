using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Seed admin if none exists
                await SeedAdminAsync(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task SeedAdminAsync(ApplicationDbContext context)
        {
            // Check if admin already exists
            if (await context.Admins.AnyAsync())
            {
                return; // Admin already exists, do nothing
            }

            string password = "admin123";
            var admin = new Admin
            {
                Email = "admin@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
