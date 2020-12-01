using System.Collections.Generic;

namespace AdvancedAnalysisDesign.Models
{
    public class PatientMedication
    {
        public int Id  { get; set; }
        public Patient Patient { get; set; }
        public Medication Medication { get; set; }
        public ICollection<Bloodwork> Bloodworks { get; set; }
    }
}