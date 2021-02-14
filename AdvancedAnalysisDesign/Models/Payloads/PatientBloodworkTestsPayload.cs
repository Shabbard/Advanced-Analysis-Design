using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AdvancedAnalysisDesign.Enums;

namespace AdvancedAnalysisDesign.Models
{
    public class PatientBloodworkTestsPayload
    {
        [Required]
        public string Result { get; set; }

        [Required]
        public string TestType { get; set; }
    }
}
