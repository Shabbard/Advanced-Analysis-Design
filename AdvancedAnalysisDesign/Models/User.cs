using AdvancedAnalysisDesign.Enums;

namespace AdvancedAnalysisDesign.Models
{
    public class User
    {
        public int Id { get; set; }
        public UserType UserType { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool Verified { get; set; }
        public UserDetail UserDetail { get; set; }
    }
}