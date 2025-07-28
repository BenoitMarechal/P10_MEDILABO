using Microsoft.EntityFrameworkCore;
using PatientMicroService.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PatientMicroService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>().HasData(
               new Patient { Id = new Guid("11111111-1111-1111-1111-111111111111"), LastName = "TestNone", FirstName = "Test", BirthDate = new DateOnly(1966, 12, 31), Gender = Gender.Female, Address = "1 Brookside St", PhoneNumber = "100-222-3333" },
               new Patient { Id = new Guid("22222222-2222-2222-2222-222222222222"), LastName = "TestBorderLine", FirstName = "Test", BirthDate = new DateOnly(1945, 06, 24), Gender = Gender.Male, Address = "2 High St", PhoneNumber = "200-333-4444" },
               new Patient { Id = new Guid("33333333-3333-3333-3333-333333333333"), LastName = "TestDanger", FirstName = "Test", BirthDate = new DateOnly(2004, 06, 18), Gender = Gender.Male, Address = "3 Club Road", PhoneNumber = "300-444-5555" },
               new Patient { Id = new Guid("44444444-4444-4444-4444-444444444444"), LastName = "TestEarlyOnSet", FirstName = "Test", BirthDate = new DateOnly(2002, 06, 28), Gender = Gender.Female, Address = "4 Valley Dr", PhoneNumber = "400-555-6666" }

    );

            // Optional: Configure enum storage
            modelBuilder.Entity<Patient>()
                .Property(p => p.Gender)
                .HasConversion<string>();
        }
    }
}
