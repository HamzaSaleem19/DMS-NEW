using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string Color { get; set; } = "#1976d2";

    [MaxLength(50)]
    public string? Icon { get; set; }

    public int? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
