namespace AdvancedAnalysisDesign.Models.Database
{
    public class Pharmacist
    {
        public int Id { get; set; }
        public User User { get; set; }
        public MedicalInstitution Pharmacy { get; set; }
    }
}