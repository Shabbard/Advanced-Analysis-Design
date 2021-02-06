using AdvancedAnalysisDesign.Enums;

namespace AdvancedAnalysisDesign.Models.Payloads
{
    public class ForgotPasswordPayload
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public string PasswordComparison { get; set; }
        public ForgotPasswordStage ForgotPasswordStage { get; set; }
        public string Token { get; set; }
    }
}
