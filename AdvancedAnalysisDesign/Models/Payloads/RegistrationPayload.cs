using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvancedAnalysisDesign.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class RegistrationPayload
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string PasswordComparison { get; set; }
        public Role UserRole { get; set; }
        [Required]
        public string NhsNumber { get; set; }
        public IList<IBrowserFile> VerificationImages { get; set; }
    }
}
