using System;
using System.Collections.Generic;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class PatientBloodwork
    {
        public int Id { get; set; }
        
        public BloodworkTest BloodworkTest { get; set; }
        public virtual ICollection<PatientBloodworkTest> PatientBloodworkTests { get; set; }
    }
}