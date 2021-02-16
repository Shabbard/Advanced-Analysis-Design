using System.Threading.Tasks;

namespace AdvancedAnalysisDesign.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}