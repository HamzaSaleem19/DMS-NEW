using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class WorkflowInstance
{
    public int Id { get; set; }

    public int WorkflowTemplateId { get; set; }
    public virtual WorkflowTemplate WorkflowTemplate { get; set; } = null!;

    public int DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;

    [Required]
    public string InitiatedById { get; set; } = string.Empty;

    public WorkflowStatus Status { get; set; } = WorkflowStatus.Pending;

    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual ICollection<WorkflowStepInstance> StepInstances { get; set; } = new List<WorkflowStepInstance>();
}

public enum WorkflowStatus
{
    Pending,
    InProgress,
    Approved,
    Rejected,
    Cancelled
}
