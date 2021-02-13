using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class PatientBloodwork
    {
        public int Id { get; set; }
        public DateTimeOffset DateOfResults { get; set; }
        public virtual ICollection<PatientBloodworkTest> PatientBloodworkTests { get; set; }
        
        public int PatientMedicationId { get; set; }
        [ForeignKey("PatientMedicationId")]
        [InverseProperty("PatientBloodworks")]
        public virtual PatientMedication PatientMedication { get; set; }
    }
}