using Microsoft.EntityFrameworkCore;
using PatientRegistrationService.Models;

namespace PatientRegistrationService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
    }
}
