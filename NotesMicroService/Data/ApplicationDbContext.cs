using Microsoft.EntityFrameworkCore;
using NotesMicroService.Models;

namespace NotesMicroService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PatientId).IsRequired();
                entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            });           

            modelBuilder.Entity<Note>().HasData(
                new Note
                {
                    Id = Guid.NewGuid(),
                    PatientId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Content = "Patient shows improvement in mobility exercises."
                },
                new Note
                {
                    Id = Guid.NewGuid(),
                    PatientId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Content = "Follow-up appointment scheduled for next week."
                },
                new Note
                {
                    Id = Guid.NewGuid(),
                    PatientId = new Guid("22222222-2222-2222-2222-222222222222"),
                    Content = "Patient reports reduced pain levels."
                }
            );
        }
    }
}