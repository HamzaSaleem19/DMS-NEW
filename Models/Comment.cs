using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class Comment
{
    public int Id { get; set; }

    public int DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public int? ParentCommentId { get; set; }
    public virtual Comment? ParentComment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }

    public bool IsEdited { get; set; } = false;

    // Navigation properties
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
