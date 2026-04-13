using MailKit.Net.Smtp;
using MimeKit;
using Noticore.Application.Interfaces;
using Polly;
using Polly.Retry;

namespace Noticore.Infrastructure.Repositories
{
    public class EmailService : IEmailService
    {
        private readonly AsyncRetryPolicy _retryPolicy; // Policy as a field to keep the code clean
        public EmailService()
        {
            _retryPolicy = Policy // Retry 3 times, waiting 2 seconds between each attempt
                .Handle<Exception>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2), (exception, timeSpan, retryCount, context) =>
                {
                    // This logic runs every time a retry is triggered
                    Console.WriteLine($"[RETRY {retryCount}] Error: {exception.Message}. Waiting {timeSpan.TotalSeconds}s...");
                });
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                // To test the retry, we could uncomment the next line:
                // throw new Exception("Temporary connection issue");

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("info@josecarlosroman.com"));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                using var client = new SmtpClient();

                // Connect to the SMTP server (using Mailtrap for testing)
                await client.ConnectAsync("sandbox.smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Authenticate with your credentials
                await client.AuthenticateAsync("mailtrap_username_here", "mailtrap_password_here");

                await client.SendAsync(email);
                await client.DisconnectAsync(true);

                Console.WriteLine($"[EMAIL SENT] To: {to} | Subject: {subject}");
                return Task.CompletedTask;
            });
        }
    }
}
