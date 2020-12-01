using System;
using System.Globalization;

namespace AdvancedAnalysisDesign.Models
{
    public class BloodworkTest
    {
        public int Id { get; set; }
        public string Results { get; set; }
        public DateTimeOffset DateOfTest { get; set; }
        public bool Completed { get; set; }
      }
}