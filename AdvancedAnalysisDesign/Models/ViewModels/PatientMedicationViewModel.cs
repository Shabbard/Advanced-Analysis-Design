using System;
using System.Collections.Generic;
using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Models.ViewModels
{
    public class PatientMedicationViewModel
    {
        public int Id { get; set; }
        public bool ShowBloodworks { get; set; }
        public string BloodworkTest { get; set; }
        public string ResultInput { get; set; }
        public Medication Medication { get; set; }
        public bool BloodworkRequired { get; set; }
        public List<PatientBloodworkViewModel> PatientBloodworks { get; set; }
        public TimeSpan DateIntervalOfBloodworkRenewal { get; set; }
        public Pickup Pickup { get; set; }
        public Patient Patient { get; set; }
    }
}