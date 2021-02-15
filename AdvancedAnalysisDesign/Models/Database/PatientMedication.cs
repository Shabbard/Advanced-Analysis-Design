using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class PatientMedication
    {
        public int Id { get; set; }
        public Medication Medication { get; set; }
        public bool BloodworkRequired { get; set; }
        public List<PatientBloodwork> PatientBloodworks { get; set; }
        public TimeSpan DateIntervalOfPickup { get; set; }
        public DateTimeOffset DateOfMedicationStart { get; set; }
        public TimeSpan DateIntervalOfBloodworkRenewal { get; set; }
        public Pickup Pickup { get; set; }
        
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        [InverseProperty("Medications")]
        public virtual Patient Patient { get; set; }
    }
}