using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public int NhsNumber { get; set; }
        public int UserId { get; set; }
        
        [ForeignKey("PatientPrescription")]
        public virtual ICollection<PatientPrescription> PrescriptionIds { get; set; }
    }
}