using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class DocumentVersion
{
    public int Id { get; set; }

    public int DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;

    public int VersionNumber { get; set; }

    [Required]
    public string FilePath { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [MaxLength(500)]
    public string? ChangeDescription { get; set; }

    [Required]
    public string CreatedById { get; set; } = string.Empty;
    public virtual ApplicationUser CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsCurrent { get; set; } = false;
}
