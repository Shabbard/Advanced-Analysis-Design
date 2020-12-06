using System;
using System.Collections.Generic;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class PatientBloodwork
    {
        public int Id { get; set; }
        public DateTimeOffset DateOfResults { get; set; }
        public virtual ICollection<PatientBloodworkTest> PatientBloodworkTests { get; set; }
    }
}