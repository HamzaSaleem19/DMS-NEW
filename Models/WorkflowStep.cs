using System.ComponentModel.DataAnnotations;

namespace DMS.Models;

public class WorkflowStep
{
    public int Id { get; set; }

    public int WorkflowTemplateId { get; set; }
    public virtual WorkflowTemplate WorkflowTemplate { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int StepOrder { get; set; }

    [MaxLength(450)]
    public string? AssignedToRoleId { get; set; }

    [MaxLength(450)]
    public string? AssignedToUserId { get; set; }

    public bool RequireAllApprovers { get; set; } = false;

    // Navigation properties
    public virtual ICollection<WorkflowStepInstance> StepInstances { get; set; } = new List<WorkflowStepInstance>();
}
