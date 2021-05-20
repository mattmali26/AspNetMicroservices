using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Email
{
    public class SendGridEmailService : IEmailService
    {
        private readonly EmailSettings settings;
        private readonly ILogger<SendGridEmailService> logger;

        public SendGridEmailService(IOptions<EmailSettings> settings, ILogger<SendGridEmailService> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        public async Task<bool> SendEmailAsync(Application.Models.Email email)
        {
            var client = new SendGridClient(settings.ApiKey);

            var subject = email.Subject;
            var to = new EmailAddress(email.To);
            var emailBody = email.Body;

            var from = new EmailAddress
            {
                Email = settings.FromAddress,
                Name = settings.FromName
            };

            var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, emailBody, emailBody);
            var response = await client.SendEmailAsync(sendGridMessage);

            logger.LogInformation("Email sent");

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            logger.LogError("Email sending failed");
            return false;
        }
    }
}