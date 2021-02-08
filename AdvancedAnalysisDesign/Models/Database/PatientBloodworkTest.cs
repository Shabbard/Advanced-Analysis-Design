using System;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class PatientBloodworkTest
    {
        public int Id { get; set; }
        public string Result { get; set; }
        public DateTimeOffset DateOfUpload { get; set; }
    }
}