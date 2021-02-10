using System;
using System.Collections.Generic;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class Medication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool BloodworkRequired { get; set; }
        public ICollection<PatientBloodwork> PatientBloodworks { get; set; }
        public TimeSpan DateIntervalOfBloodworkRenewal { get; set; }
        public Pickup Pickup { get; set; }
    }
}