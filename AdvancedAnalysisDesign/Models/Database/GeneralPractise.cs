namespace AdvancedAnalysisDesign.Models.Database
{
    public class GeneralPractise : IMedicalInstitution
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
    }
}