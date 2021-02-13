using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Models.DataTransferObjects
{
    public class PatientMedicationsDto : PatientMedication
    {
        public bool ShowDetails { get; set; }
        public string BloodworkTest { get; set; }
        public string ResultInput { get; set; }
    }
}