using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models
{
    public class Bloodwork
    {
        public int Id { get; set; }
        public DateTimeOffset DateOfResults { get; set; }
        
        [ForeignKey("BloodworkTest")]
        public virtual ICollection<BloodworkTest> BloodworkTestIds { get; set; }
    }
}