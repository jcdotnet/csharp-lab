using MailKit.Net.Smtp;
using MimeKit;
using Noticore.Application.Interfaces;
using Polly;
using Polly.Retry;

namespace Noticore.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly ResiliencePipeline _pipeline; //  Resilience Policy 
        public SmtpEmailService()
        {
            _pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = args =>
                    {
                        // This logic runs every time a retry is triggered
                        var retryCount = args.AttemptNumber + 1;
                        var exception = args.Outcome.Exception?.Message;
                        var totalSeconds = args.RetryDelay.TotalSeconds;

                        Console.WriteLine($"[RETRY {retryCount}] Error: {exception}. Waiting {totalSeconds}s...");

                        return default;
                    }
                })
                .Build();
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _pipeline.ExecuteAsync(async _ =>
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
                await client.ConnectAsync("sandbox.smtp.mailtrap.io", 587,
                    MailKit.Security.SecureSocketOptions.StartTls);

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
