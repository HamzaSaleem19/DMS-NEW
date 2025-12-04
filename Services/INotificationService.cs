using DMS.Models;

namespace DMS.Services;

public interface INotificationService
{
    Task CreateNotificationAsync(string userId, string title, string message, NotificationType type, int? relatedEntityId = null, string? relatedEntityType = null);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, bool unreadOnly = false);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task DeleteNotificationAsync(int notificationId);
}
