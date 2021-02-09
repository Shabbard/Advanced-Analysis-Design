﻿using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class PatientImages
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] ImageUrl { get; set; }
    }
}
