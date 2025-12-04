using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class Notification
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public int? RelatedEntityId { get; set; }

    [MaxLength(100)]
    public string? RelatedEntityType { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}

public enum NotificationType
{
    DocumentUploaded,
    DocumentShared,
    DocumentUpdated,
    DocumentDeleted,
    WorkflowAssigned,
    WorkflowApproved,
    WorkflowRejected,
    CommentAdded,
    PermissionGranted,
    PermissionRevoked
}
