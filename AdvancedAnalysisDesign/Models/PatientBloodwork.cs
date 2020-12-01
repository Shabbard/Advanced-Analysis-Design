using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models
{
    public class PatientBloodwork
    {
        public int Id { get; set; }
        public DateTimeOffset DateOfResults { get; set; }
        public virtual ICollection<PatientBloodworkTest> PatientBloodworkTests { get; set; }
    }
}