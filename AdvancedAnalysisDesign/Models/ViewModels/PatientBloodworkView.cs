using System.Collections.Generic;
using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Models.ViewModels
{
    public class PatientBloodworkView
    {
        public BloodworkTest BloodworkTest { get; set; }
        public PatientMedication PatientMedication { get; set; }
        public List<PatientBloodworkTestView> PatientBloodworkTests { get; set; }
    }
}