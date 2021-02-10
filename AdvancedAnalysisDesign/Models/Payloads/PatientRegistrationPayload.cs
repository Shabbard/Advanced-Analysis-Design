using System;
using System.ComponentModel.DataAnnotations;
using AdvancedAnalysisDesign.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class PatientRegistrationPayload : RegistrationPayload
    {
        [Required]
        public string NhsNumber { get; set; }
        [Required]
        public IBrowserFile IDPhoto { get; set; }
        public IBrowserFile SelfiePhoto { get; set; }
    }
}