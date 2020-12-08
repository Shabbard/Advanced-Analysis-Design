﻿using System.Net;
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
                       "Thanks Binary Beast Bloodwork";

            var client = new SmtpClient("smtp.gmail.com",587)
            {
                Credentials = new NetworkCredential(binaryBeastEmailAddress, EmailPassword),
                EnableSsl = true
            };
            
            client.Send(binaryBeastEmailAddress,emailAddress,Subject,Body);
        }
    }
}