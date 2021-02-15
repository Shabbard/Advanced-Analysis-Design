using System.Collections.Generic;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class Patient
    {
        public int Id { get; set; }
        public string NhsNumber { get; set; }
        public User User { get; set; }
        public GeneralPractitioner GeneralPractitioner { get; set; }
        public MedicalInstitution MedicalInstitution { get; set; }
        public PatientImages PatientImages { get; set; }
        public List<PatientMedication> Medications { get; set; }
    }
}