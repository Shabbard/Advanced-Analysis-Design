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

        public async Task SendForgotPasswordEmail(string emailAddress, string firstName, int? token)
        {

            var EmailPassword = Configuration["EmailPassword"];
            const string binaryBeastEmailAddress = "binarybeastbloodwork@gmail.com";

            var Subject = "Forgot Password Code";
            var Body = "Hi " + firstName + ",\n\n" +
                       "Your unique code is " + token + ". It will expire in 5 minutes.\n\n" +
                       "You're receiving this email because a request to reset your password was received.\n" +
                       "If you did not request this code, please disregard this email.\n\n" +
                       "Have a nice day.\n" +
                       "Binary Beast Bloodwork";

            var client = new SmtpClient("smtp.gmail.com",587)
            {
                Credentials = new NetworkCredential(binaryBeastEmailAddress, EmailPassword),
                EnableSsl = true
            };
            
            client.Send(binaryBeastEmailAddress,emailAddress,Subject,Body);
        }
        
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