namespace AdvancedAnalysisDesign.Models.Database
{
    public class Surgery : IMedicalInstitution
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
    }
}