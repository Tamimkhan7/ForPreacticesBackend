using System.Net;
using System.Net.Mail;

namespace ForPractices.Service.Email
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
            var host = _config["smtp:Host"];
            var port = int.Parse(_config["smtp:Port"]);
            var email = _config["smtp:Email"];
            var password = _config["smtp:Password"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true
            };


            using var mail = new MailMessage
            {
                From = new MailAddress(email),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);

        }
    }
}
