using MediatR;
using Noticore.Application.Interfaces;
using Noticore.Domain.Enums;

namespace Noticore.Application.Notifications.Events
{
    public class EmailNotificationHandler(
        IEmailService emailService,
        INotificationRepository repository
      ) : INotificationHandler<NotificationCreatedEvent>
    {
        public async Task Handle(NotificationCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Logic to send email only if the type is Email
            if (notification.Notification.Type == Domain.Enums.NotificationType.Email)
            {
                try
                {
                    await emailService.SendEmailAsync(
                    notification.Notification.Recipient,
                    notification.Notification.Title,
                    notification.Notification.Message);

                    notification.Notification.Status = NotificationStatus.Sent;
                }
                catch (Exception)
                {
                    notification.Notification.Status = NotificationStatus.Failed;
                }

                await repository.UpdateAsync(notification.Notification);
            }
        }
    }
}
