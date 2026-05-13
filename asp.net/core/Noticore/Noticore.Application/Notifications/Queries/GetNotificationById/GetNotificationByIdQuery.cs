using MediatR;
using Noticore.Application.Interfaces;
using Noticore.Application.Notifications.Dtos;

namespace Noticore.Application.Notifications.Queries.GetNotificationById
{
    // The Query: We need the ID to find the specific notification
    public record GetNotificationByIdQuery(Guid Id) : IRequest<NotificationDto?>;

    // The Handler: Uses the repository to fetch the entity
    public class GetNotificationByIdHandler(INotificationRepository repository)
        : IRequestHandler<GetNotificationByIdQuery, NotificationDto?>
    {
        public async Task<NotificationDto?> Handle(GetNotificationByIdQuery request, 
            CancellationToken cancellationToken)
        {
            var n = await repository.GetByIdAsync(request.Id);
            if (n == null) return null;

            return new NotificationDto(
                n.Id, n.Title, n.Message, n.Recipient,
                n.Type.ToString(), n.Status.ToString(), n.CreatedAt);
        }
    }
}
