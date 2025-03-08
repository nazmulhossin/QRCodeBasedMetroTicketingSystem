using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Station> Stations { get; set; }
        public DbSet<StationDistance> StationDistances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}
