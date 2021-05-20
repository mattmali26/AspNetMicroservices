using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Email
{
    public class MailKitEmailService : IEmailService
    {
        private readonly EmailSettings settings;
        private readonly ILogger<MailKitEmailService> logger;

        public MailKitEmailService(IOptions<EmailSettings> settings, ILogger<MailKitEmailService> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        public async Task<bool> SendEmailAsync(Application.Models.Email email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));
            message.To.Add(MailboxAddress.Parse(email.To));
            message.Subject = email.Subject;

            message.Body = new TextPart("plain")
            {
                Text = email.Body
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.friends.com", 587, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("joey", "password");

                await client.SendAsync(message);
                client.Disconnect(true);

                logger.LogInformation("Email sent");

                return true;
            }
        }
    }
}