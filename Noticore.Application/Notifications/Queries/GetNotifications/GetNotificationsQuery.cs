using MediatR;
using Noticore.Application.Interfaces;
using Noticore.Domain.Entities;

namespace Noticore.Application.Notifications.Queries.GetNotifications
{
    // The Query: No data needed to get all notifications
    public record GetNotificationsQuery() : IRequest<IEnumerable<Notification>>;

    // The Handler: Fetches data from the repository (using primary ctor to inject the repo)
    public class GetNotificationsHandler(INotificationRepository repository)
        : IRequestHandler<GetNotificationsQuery, IEnumerable<Notification>>
    {
        public async Task<IEnumerable<Notification>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetAllAsync();
        }
    }
}
