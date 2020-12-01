using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign
{
    public class AADContext : DbContext
    {
        public AADContext(DbContextOptions<AADContext> options)
            : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedication> PatientMedications { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PatientBloodwork> PatientBloodworks { get; set; }
        public DbSet<PatientBloodworkTest> PatientBloodworkTests { get; set; }
    }
}