using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Bloodwork Bloodwork { get; set; }
    }
}