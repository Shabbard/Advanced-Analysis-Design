namespace AdvancedAnalysisDesign.Models
{
    public class PatientPrescription
    {
        public int Id  { get; set; }
        public Patient Patient { get; set; }
        public Prescription Prescription { get; set; }
}
}