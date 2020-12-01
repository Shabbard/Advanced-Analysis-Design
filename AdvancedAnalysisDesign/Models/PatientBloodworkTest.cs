using System;
using System.Globalization;

namespace AdvancedAnalysisDesign.Models
{
    public class PatientBloodworkTest
    {
        public int Id { get; set; }
        public string Results { get; set; }
        public bool Completed { get; set; }
      }
}