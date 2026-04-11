using HooperMedia.Core.BusinessRules;
using HooperMedia.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HooperMedia.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Person> Persons { get; set; } = null!;

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
           
            });
        
        }
    }
}
