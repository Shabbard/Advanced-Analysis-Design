using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Models.DataTransferObjects
{
    public class UserWithRoleDto
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}