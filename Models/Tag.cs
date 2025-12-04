using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Color { get; set; } = "#607d8b";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
}
