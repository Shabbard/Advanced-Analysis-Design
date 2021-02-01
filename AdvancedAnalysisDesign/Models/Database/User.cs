using AdvancedAnalysisDesign.Enums;
using Microsoft.AspNetCore.Identity;

namespace AdvancedAnalysisDesign.Models.Database
{
    public class User : IdentityUser
    {
        public UserDetail UserDetail { get; set; }
    }
}