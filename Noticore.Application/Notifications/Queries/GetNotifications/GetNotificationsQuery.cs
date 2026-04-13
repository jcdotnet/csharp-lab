using MediatR;
using Noticore.Application.Interfaces;
using Noticore.Application.Notifications.Dtos;

namespace Noticore.Application.Notifications.Queries.GetNotifications
{
    // The Query: No data needed to get all notifications
    public record GetNotificationsQuery() : IRequest<IEnumerable<NotificationDto>>;

    // The Handler: Fetches data from the repository (using primary ctor to inject the repo)
    public class GetNotificationsHandler(INotificationRepository repository)
        : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
    {
        public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsQuery request, 
            CancellationToken cancellationToken)
        {
            // return await repository.GetAllAsync();

            var notifications = await repository.GetAllAsync();

            return notifications.Select(n => new NotificationDto(
                n.Id,
                n.Title,
                n.Message,
                n.Recipient,
                n.Type.ToString(),
                n.Status.ToString(),
                n.CreatedAt
            ));
        }
    }
}
