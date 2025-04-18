using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Station> Stations { get; set; }
        public DbSet<StationDistance> StationDistances { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Station>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<Station>()
                .HasIndex(s => s.Order)
                .IsUnique();

            modelBuilder.Entity<StationDistance>()
                .HasOne(sd => sd.Station1)
                .WithMany()
                .HasForeignKey(sd => sd.Station1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StationDistance>()
                .HasOne(sd => sd.Station2)
                .WithMany()
                .HasForeignKey(sd => sd.Station2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SystemSettings>().HasData(
                new SystemSettings
                {
                    Id = 1,
                    MinFare = 20.0000m,
                    FarePerKm = 5.0000m,
                    RapidPassQrCodeValidityMinutes = 1440,
                    QrTicketValidityMinutes = 2880,
                    MaxTripDurationMinutes = 120,
                    TimeLimitPenaltyFee = 100.00m,
                    CreatedAt = new DateTime(2025, 03, 14, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 03, 14, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<User>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(s => s.PhoneNumber)
                .IsUnique();
        }
    }
}
