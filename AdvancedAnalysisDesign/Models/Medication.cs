using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models
{
    public class Medication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool BloodworkRequired { get; set; }
    }
}