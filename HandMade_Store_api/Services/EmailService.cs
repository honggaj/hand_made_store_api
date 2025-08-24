using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FurnitureStoreAPI.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FurnitureStoreAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpConfig = _config.GetSection("Smtp");
            string host = smtpConfig["Host"];
            int port = int.Parse(smtpConfig["Port"]);
            string username = smtpConfig["Username"];
            string password = smtpConfig["Password"];
            bool enableSsl = bool.Parse(smtpConfig["EnableSsl"]);

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(username, "Furniture Store"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}
