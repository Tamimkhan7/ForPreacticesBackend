using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> SendEmailAsync(string to, string subject, string body)
        {
            var host = _config["smtp:Host"];
            var port = _config["smtp:Port"];
            var email = _config["smtp:Email"];
            var password = _config["smtp:Password"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true
            };


        }
    }
}
