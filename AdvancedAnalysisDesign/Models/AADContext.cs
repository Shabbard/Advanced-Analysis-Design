using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign.Models
{
    public class AADContext : DbContext
    {
        public AADContext (
            DbContextOptions<AADContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientPrescription> PatientPrescriptions { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Bloodwork> Bloodworks { get; set; }
        public DbSet<BloodworkTest> BloodworkTests { get; set; }
    }
}