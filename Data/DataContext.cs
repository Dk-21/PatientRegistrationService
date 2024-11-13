using Microsoft.EntityFrameworkCore;
using PatientRegistrationService.Models;

namespace PatientRegistrationService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(p => p.Name).HasMaxLength(100);
                entity.Property(p => p.MedicalRecordNumber).HasMaxLength(50);
                entity.Property(p => p.Gender).HasMaxLength(20);
                entity.Property(p => p.AttendingPhysician).HasMaxLength(100);
                entity.Property(p => p.Department).HasMaxLength(50);
            });
        }
    }
}
