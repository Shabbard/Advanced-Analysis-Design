using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Services
{
    public interface ISignInService
    {
        Task SignInAsync(User user);

        Task SignOutAsync();

        Task ResetUserLoginAsync(User user);
    }
}