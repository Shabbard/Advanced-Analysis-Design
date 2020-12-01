using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public int NhsNumber { get; set; }
        public User User { get; set; }
        public byte[] VerificationImage { get; set; }
    }
}