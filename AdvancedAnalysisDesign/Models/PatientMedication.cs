using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdvancedAnalysisDesign.Models
{
    public class PatientMedication
    {
        public int Id  { get; set; }
        public Patient Patient { get; set; }
        public Medication Medication { get; set; }
        public ICollection<PatientBloodwork> PatientBloodworks { get; set; }
        public TimeSpan DateIntervalOfBloodworkRenewal { get; set; }
    }
}