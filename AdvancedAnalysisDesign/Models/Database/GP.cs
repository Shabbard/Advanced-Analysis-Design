﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class GP
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string EmergencyContact { get; set; }
        public string OfficeNumber  { get; set; }

    }
}
