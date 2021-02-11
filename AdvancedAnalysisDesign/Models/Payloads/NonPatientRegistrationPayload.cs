using System;
using System.ComponentModel.DataAnnotations;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;
using Microsoft.AspNetCore.Components.Forms;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class NonPatientRegistrationPayload : RegistrationPayload
    {
        public MedicalInstitution MedicalInstitution { get; set; }
    }
}