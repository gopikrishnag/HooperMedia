using HooperMedia.Core.BusinessRules;
using HooperMedia.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HooperMedia.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Person entity
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(p => p.PersonId);

                entity.Property(p => p.PersonId)
                    .ValueGeneratedOnAdd();

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(PersonBusinessRules.NameMaxLength);

                entity.Property(p => p.DateOfBirth)
                    .IsRequired();

                entity.HasMany(p => p.Addresses)
                    .WithOne(a => a.Person)
                    .HasForeignKey(a => a.PersonId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Persons", t => t.HasComment("Stores person information"));
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.AddressId);

                entity.Property(a => a.AddressId)
                    .ValueGeneratedOnAdd();

                entity.Property(a => a.PersonId)
                    .IsRequired();

                entity.Property(a => a.AddressLine1)
                    .IsRequired()
                    .HasMaxLength(AddressBusinessRules.AddressLineMaxLength);

                entity.Property(a => a.AddressLine2)
                    .HasMaxLength(AddressBusinessRules.AddressLineMaxLength);

                entity.Property(a => a.TownOrCity)
                    .IsRequired()
                    .HasMaxLength(AddressBusinessRules.AddressLineMaxLength);

                entity.Property(a => a.ZipOrPostCode)
                    .IsRequired()
                    .HasMaxLength(AddressBusinessRules.ZipOrPostCodeMaxLength);

                entity.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(AddressBusinessRules.CountryMaxLength);

                entity.HasOne(a => a.Person)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(a => a.PersonId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Addresses");

                entity.HasIndex(a => a.PersonId)
                    .HasDatabaseName("IX_Addresses_PersonId");
            });

        }
    }
}
