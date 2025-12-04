using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class WorkflowStepInstance
{
    public int Id { get; set; }

    public int WorkflowInstanceId { get; set; }
    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;

    public int WorkflowStepId { get; set; }
    public virtual WorkflowStep WorkflowStep { get; set; } = null!;

    public StepStatus Status { get; set; } = StepStatus.Pending;

    public string? AssignedToUserId { get; set; }

    [MaxLength(500)]
    public string? Comments { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public enum StepStatus
{
    Pending,
    InProgress,
    Approved,
    Rejected,
    Skipped
}
