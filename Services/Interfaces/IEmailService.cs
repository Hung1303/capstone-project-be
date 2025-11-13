namespace Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body);
        Task<bool> SendEmailToCentersAsync(List<string> centerEmails, string subject, string body);
    }
}

