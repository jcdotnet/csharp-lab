using Noticore.Domain.Entities;

namespace Noticore.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(Guid id);
        Task<IEnumerable<Notification>> GetAllAsync();
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);

        // Note: Delete operation is intentionally omitted to maintain notification history and audit trail. 
        // If removal is required, consider implementing a Soft Delete (IsDeleted) approach.
    }
}
