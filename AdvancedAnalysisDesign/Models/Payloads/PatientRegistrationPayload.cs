using System;
using System.ComponentModel.DataAnnotations;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;
using Microsoft.AspNetCore.Components.Forms;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class PatientRegistrationPayload : RegistrationPayload
    {
        [Required]
        public string NhsNumber { get; set; }
        [Required]
        public IBrowserFile IDPhoto { get; set; }
        [Required]
        public IBrowserFile SelfiePhoto { get; set; }
        public MedicalInstitution MedicalInstitution { get; set; }
    }
}