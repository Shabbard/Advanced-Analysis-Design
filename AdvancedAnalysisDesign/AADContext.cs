using AdvancedAnalysisDesign.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign
{
    public class AADContext : IdentityDbContext<User>
    {
        public AADContext(DbContextOptions<AADContext> options)
            : base(options)
        {
        }

        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PatientBloodwork> PatientBloodworks { get; set; }
        public DbSet<PatientBloodworkTest> PatientBloodworkTests { get; set; }
        public DbSet<BloodworkTest> BloodworkTests { get; set; }
        public DbSet<GeneralPractitioner> GeneralPractitioners { get; set; }
        public DbSet<PatientImages> PatientImages { get; set; }
        public DbSet<Pickup> Pickups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
