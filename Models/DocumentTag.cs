namespace DMS.Models;

public class DocumentTag
{
    public int DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;

    public int TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
