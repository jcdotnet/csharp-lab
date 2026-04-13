using MediatR;
using Noticore.Application.Interfaces;
using Noticore.Domain.Entities;

namespace Noticore.Application.Notifications.Queries.GetNotificationById
{
    // The Query: We need the ID to find the specific notification
    public record GetNotificationByIdQuery(Guid Id) : IRequest<Notification?>;

    // The Handler: Uses the repository to fetch the entity
    public class GetNotificationByIdHandler(INotificationRepository repository)
        : IRequestHandler<GetNotificationByIdQuery, Notification?>
    {
        public async Task<Notification?> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetByIdAsync(request.Id);
        }
    }
}
