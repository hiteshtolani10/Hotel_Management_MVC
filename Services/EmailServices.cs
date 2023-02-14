using Hotel_Management_MVC.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Hotel_Management_MVC.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly SMTPConfigModels _smtpConfig;
        string fromEmail = "hiteshtolani1018@gmail.com";
        private string mailPassword = "Hitesh@1018";


        public EmailServices(IOptions<SMTPConfigModels> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }

        private const string templatePath = @"EmailTemplates/{0}.html";

        public async Task SendTestEmail(UserEmailOptions emailOptions)
        {
            emailOptions.Subject = "This is a test Email";
            emailOptions.Body = GetEmailBody("TestEmail");

            await SendEmail(emailOptions);
        }

        private async Task SendEmail(UserEmailOptions emailOptions)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    Subject = emailOptions.Subject,
                    Body = emailOptions.Body,
                    From = new MailAddress(fromEmail, _smtpConfig.SenderDisplayName),
                    IsBodyHtml = true
                };

                foreach (var toEmail in emailOptions.ToEmails)
                {
                    mail.To.Add(new MailAddress(toEmail));
                }


                var smtpClient = new SmtpClient
                {
                    Host = "smtp.office365.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail, mailPassword, "office365.com")
            };

            smtpClient.Send(mail);

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }

        }

        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }
    }
}
