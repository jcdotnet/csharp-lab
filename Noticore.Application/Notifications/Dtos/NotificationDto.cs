namespace Noticore.Application.Notifications.Dtos
{
    // In Clean Architecture, we never return Domain Entities directly to the API client. Instead, we use DTOs
    
    public record NotificationDto( // Using a record for the DTO as it's just a data carrier
        Guid Id,
        string Title,
        string Message,
        string Recipient,
        string Type, // We'll convert the Enum to string for the client
        string Status,
        DateTime CreatedAt
    );
}
