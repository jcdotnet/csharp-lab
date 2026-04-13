using Noticore.Application.Interfaces;

namespace Noticore.Infrastructure.Repositories
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine($"[EMAIL SENT] To: {to} | Subject: {subject}");
            return Task.CompletedTask;
        }
    }
}
