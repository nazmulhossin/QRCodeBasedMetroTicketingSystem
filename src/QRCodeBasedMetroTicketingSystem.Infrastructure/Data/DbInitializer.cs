using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                var configuration = services.GetRequiredService<IConfiguration>();

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Seed admin if none exists
                await SeedAdminAsync(context, configuration);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task SeedAdminAsync(ApplicationDbContext context, IConfiguration configuration)
        {
            // Check if admin already exists
            if (await context.Admins.AnyAsync())
            {
                return; // Admin already exists, do nothing
            }

            var admin = new Admin
            {
                Email = configuration["AdminSettings:DefaultEmail"]!,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(configuration["AdminSettings:DefaultPassword"]),
                CreatedAt = DateTime.UtcNow
            };

            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
