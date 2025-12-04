using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class Document
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FileExtension { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [MaxLength(100)]
    public string MimeType { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;

    [Required]
    public string UploadedById { get; set; } = string.Empty;
    public virtual ApplicationUser UploadedBy { get; set; } = null!;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }

    public int CurrentVersion { get; set; } = 1;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Draft;

    [MaxLength(32)]
    public string? CheckedOutBy { get; set; }
    public DateTime? CheckedOutAt { get; set; }

    // Navigation properties
    public virtual ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    public virtual ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
    public virtual ICollection<DocumentPermission> Permissions { get; set; } = new List<DocumentPermission>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();
}

public enum DocumentStatus
{
    Draft,
    PendingReview,
    Approved,
    Rejected,
    Archived,
    Published
}
