using System.Collections.Generic;
using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Models.ViewModels
{
    public class PatientBloodworkViewModel
    {
        public int Id { get; set; }
        public bool ShowDetails { get; set; }
        public BloodworkTest BloodworkTest { get; set; }
        public PatientMedication PatientMedication { get; set; }
        public List<PatientBloodworkTestViewModel> PatientBloodworkTests { get; set; }
    }
}