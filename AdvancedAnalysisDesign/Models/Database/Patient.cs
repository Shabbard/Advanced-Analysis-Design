namespace AdvancedAnalysisDesign.Models.Database
{
    public class Patient
    {
        public int Id { get; set; }
        public string NhsNumber { get; set; }
        public User User { get; set; }
        public GeneralPractitioner GeneralPractitioner { get; set; }
        public byte[] VerificationImage { get; set; }
    }
}