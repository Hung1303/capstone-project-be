using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Services.Interfaces;

namespace Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"] ?? "TMS Admin";

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || 
                    string.IsNullOrEmpty(smtpPassword) || string.IsNullOrEmpty(fromEmail))
                {
                    _logger.LogError("Email configuration is missing");
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendEmailToCentersAsync(List<string> centerEmails, string subject, string body)
        {
            if (centerEmails == null || !centerEmails.Any())
            {
                _logger.LogWarning("No center emails provided");
                return false;
            }

            var successCount = 0;
            var failedEmails = new List<string>();

            foreach (var email in centerEmails)
            {
                if (string.IsNullOrWhiteSpace(email))
                    continue;

                try
                {
                    var result = await SendEmailAsync(email, "Trung tâm", subject, body);
                    if (result)
                    {
                        successCount++;
                    }
                    else
                    {
                        failedEmails.Add(email);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send email to {email}");
                    failedEmails.Add(email);
                }
            }

            _logger.LogInformation($"Sent {successCount} out of {centerEmails.Count} emails. Failed: {failedEmails.Count}");
            
            return successCount > 0;
        }
    }
}

