using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AdvancedAnalysisDesign.Services
{
    public class EmailService
    {
        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailPassword = Configuration["EmailPassword"];
            const string binaryBeastEmailAddress = "binarybeastbloodwork@gmail.com";
            
            var client = new SmtpClient("smtp.gmail.com",587)
            {
                Credentials = new NetworkCredential(binaryBeastEmailAddress, emailPassword),
                EnableSsl = true
            };
            
            await client.SendMailAsync(binaryBeastEmailAddress,email,subject,htmlMessage);
        }
    }
}