using Noticore.Application.Interfaces;

namespace Noticore.Infrastructure.Services
{
    public class FakeEmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "SentEmails");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
            var fileName = $"{DateTime.Now:HHmmss}_{to}.html";
            var filePath = Path.Combine(folderPath, fileName);

            Console.WriteLine($"[FAKE EMAIL] Guardando email para {to} en {filePath}...");
            await File.WriteAllTextAsync(filePath, body);
        }
    }

}
