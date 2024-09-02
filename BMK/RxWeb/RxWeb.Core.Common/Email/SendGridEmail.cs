using RxWeb.Core.Common.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RxWeb.Core.Common.Email
{
    public class SendGridEmail : IEmail
    {

        public SendGridEmail(SendGridEmailConfiguration emailConfiguration)
        {
            EmailConfiguration = emailConfiguration;
            Configure();
        }

        public async Task<bool> SendAsync(MailConfig config)
        {
            var mailMessage = GetMailMessage(config);
            var res = await SendClient.SendEmailAsync(mailMessage);
            return res.IsSuccessStatusCode;
        }

        private SendGridMessage GetMailMessage(MailConfig config)
        {
            var from = new EmailAddress(config.From);
            var tos = new List<EmailAddress>();
            config.To.ForEach(t => tos.Add(new EmailAddress(t)));
            var htmlContent = string.Empty;
            var textContent = string.Empty;
            var templateData = new object();
            if (config.EmailFormat == EmailFormatType.Html)
                htmlContent = config.Body;
            else if (config.EmailFormat == EmailFormatType.TemplatedId)
            {
                templateData = config.TemplateData;
            }
            else
                textContent = config.Body;
            SendGridMessage mailMessage;
            if (config.EmailFormat == EmailFormatType.TemplatedId)
                mailMessage = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from, tos, config.TemplateId, templateData);
            else
                mailMessage = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, config.Subject, textContent, htmlContent);
            AddAttachments(config, mailMessage);
            return mailMessage;
        }

        private void AddAttachments(MailConfig config, SendGridMessage mailMessage)
        {
            foreach (var attachment in config.Attachments)
                mailMessage.AddAttachmentAsync(attachment.Key, attachment.Value);
        }

        private void Configure()
        {
            SendClient = new SendGridClient(EmailConfiguration.ApiKey);
        }

        private SendGridEmailConfiguration EmailConfiguration { get; set; }
        private SendGridClient SendClient { get; set; }
    }
}
