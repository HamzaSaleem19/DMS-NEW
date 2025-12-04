using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class DocumentPermission
{
    public int Id { get; set; }

    public int DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;

    [MaxLength(450)]
    public string? UserId { get; set; }

    [MaxLength(450)]
    public string? RoleId { get; set; }

    public PermissionType PermissionType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}

public enum PermissionType
{
    View,
    Edit,
    Delete,
    Share,
    FullControl
}
