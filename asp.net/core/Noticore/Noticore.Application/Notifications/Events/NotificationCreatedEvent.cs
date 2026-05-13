using MediatR;
using Noticore.Domain.Entities;

namespace Noticore.Application.Notifications.Events
{
    // An event is just a notification (in MediatR terms) that something happened
    public record NotificationCreatedEvent(Notification Notification) : INotification;
}
