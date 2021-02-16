using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class PatientBloodworkTest
    {
        public int Id { get; set; }
        public bool Result { get; set; }
        public DateTimeOffset DateOfUpload { get; set; }
        public string TestType { get; set; }

        public int PatientBloodworkId { get; set; }
        [ForeignKey("PatientBloodworkId")]
        [InverseProperty("PatientBloodworkTests")]
        public virtual PatientBloodwork PatientBloodwork { get; set; }
    }
}