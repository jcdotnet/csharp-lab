using CitiesManager.Core.Entities;
using CitiesManager.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContext
{
    // public class ApplicationDbContext : DbContext
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {

        // maybe to remove default constructor
        public ApplicationDbContext()
        {
            
        }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
           
        }

        // DbSets for our domains
        public virtual DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<CityDetail> CityDetails { get; set; } = null!;
        public DbSet<Citizen> Citizens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring tables
            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Countries"); // optional but recommended
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            });
            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("Cities");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(c => c.Country)
                      .WithMany(co => co.Cities)
                      .HasForeignKey(c => c.CountryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CityDetail>(entity =>
            {
                entity.ToTable("CityDetails");

                entity.HasKey(cd => cd.CityId); // both the Primary Key and the Foreign Key (Strict 1-to-1)

                entity.Property(cd => cd.MayorName).IsRequired().HasMaxLength(150);
                
                entity.HasOne(cd => cd.City)
                      .WithOne(c => c.Detail)
                      .HasForeignKey<CityDetail>(cd => cd.CityId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Citizen>(entity =>
            {
                entity.ToTable("Citizens");
                entity.HasKey(cz => cz.Id);

                entity.Property(cz => cz.FullName).IsRequired().HasMaxLength(150);
                entity.Property(cz => cz.Address).IsRequired().HasMaxLength(250);
                entity.Property(cz => cz.DateOfBirth).IsRequired();

                entity.HasOne(cz => cz.City)
                      .WithMany(c => c.Citizens)
                      .HasForeignKey(cz => cz.CityId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            var spainId = Guid.Parse("A1B2C3D4-E5F6-7A8B-9C0D-E1F2A3B4C5D6");
            var ukId = Guid.Parse("B2C3D4E5-F6A7-8B9C-0D1E-2F3A4B5C6D7E");

            modelBuilder.Entity<Country>().HasData(
                new Country { Id = spainId, Name = "Spain" },
                new Country { Id = ukId, Name = "United Kingdom" }
            );

            modelBuilder.Entity<City>().HasData(new City()
            {
                Id = Guid.Parse("EB65F42D-C9C4-4997-86D9-FC31795081EE"),
                Name = "Madrid",
                CountryId = spainId
            });
            modelBuilder.Entity<City>().HasData(new City() { 
                Id = Guid.Parse("0DBF624E-7440-463F-A68E-0BC058DDF407"),
                Name = "Málaga",
                CountryId = spainId
            });
            modelBuilder.Entity<City>().HasData(new City()
            {
                Id = Guid.Parse("21442586-D075-4948-ADBF-1BD0C443A046"),
                Name = "London",
                CountryId = ukId
            });
        }
    }
}
