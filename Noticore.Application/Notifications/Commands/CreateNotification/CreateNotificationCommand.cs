using MediatR;
using Noticore.Application.Interfaces;
using Noticore.Application.Notifications.Events;
using Noticore.Domain.Entities;
using Noticore.Domain.Enums;

namespace Noticore.Application.Notifications.Commands.CreateNotification
{
    // The command: Defines the data needed to perform the action
    // Using 'record' is a best practice for DTOs/Commands as they are immutable
    public record CreateNotificationCommand(
    string Title,
    string Message,
    string Recipient,
    NotificationType Type) : IRequest<Guid>; // We expect the ID of the created notification as a result

    // The handler: Contains the business logic to process the Command. This now acts as a Publisher too: 
    // After persisting the notification, it triggers a Domain Event to decouple side effects (sending emails).
    public class CreateNotificationHandler : IRequestHandler<CreateNotificationCommand, Guid>
    {
        private readonly INotificationRepository _repository;
        private readonly IPublisher _publisher;

        public CreateNotificationHandler(INotificationRepository repository, IPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            // Mapping Command to Domain Entity
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Message = request.Message,
                Recipient = request.Recipient,
                Type = request.Type,
                CreatedAt = DateTime.UtcNow
            };

            // Persistence via Repository
            await _repository.AddAsync(notification);

            // We publish the event. We don't wait for the email to finish here!
            await _publisher.Publish(new NotificationCreatedEvent(notification), cancellationToken);

            return notification.Id;
        }
    }
}
