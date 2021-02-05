using System.ComponentModel.DataAnnotations;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class UpdatePasswordPayload
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }
}