using System.ComponentModel.DataAnnotations;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class AddMedicalInstitutionPayload
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Phone]
        public string ContactNumber { get; set; }
    }
}