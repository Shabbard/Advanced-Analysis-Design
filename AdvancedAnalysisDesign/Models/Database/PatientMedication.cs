using System;
using System.Collections.Generic;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class PatientMedication
    {
        public int Id  { get; set; }
        public Patient Patient { get; set; }
        public Medication Medication { get; set; }
    }
}