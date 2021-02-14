using System;
using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Models.ViewModels
{
    public class PatientBloodworkTestView
    {
        
        public string Result { get; set; }
        public DateTimeOffset DateOfUpload { get; set; }
        public string TestType { get; set; }
        public PatientBloodwork PatientBloodwork { get; set; }
    }
}