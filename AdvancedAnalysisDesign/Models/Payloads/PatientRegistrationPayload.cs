using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class PatientRegistrationPayload
    {
        [Required]
        public string NhsNumber { get; set; }
        [Required]
        public IBrowserFile IDPhoto { get; set; }
        public IBrowserFile SelfiePhoto { get; set; }
    }
}