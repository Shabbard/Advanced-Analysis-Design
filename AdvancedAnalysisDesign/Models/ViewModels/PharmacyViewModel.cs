using System.Collections.Generic;
using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Models.ViewModels
{
    public class PharmacyViewModel
    {
        public int Id { get; set; }
        public bool ShowDetails { get; set; }
        public UserDetail UserDetail { get; set; }
        public List<PatientMedication> PatientMedication { get; set; }
    }
}