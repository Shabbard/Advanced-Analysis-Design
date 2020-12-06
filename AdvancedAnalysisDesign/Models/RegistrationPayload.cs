using System;
using System.ComponentModel.DataAnnotations;
using AdvancedAnalysisDesign.Enums;

namespace AdvancedAnalysisDesign.Models
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
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string PasswordComparison { get; set; }
        public UserType UserType { get; set; }
        [Required]
        public string NhsNumber { get; set; }
        public byte[] VerificationImage { get; set; }
    }
}
