using Microsoft.EntityFrameworkCore;
using DMS.Data;
using DMS.Models;

namespace DMS.Services;

public class WorkflowService : IWorkflowService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IAuditService _auditService;

    public WorkflowService(ApplicationDbContext context, INotificationService notificationService, IAuditService auditService)
    {
        _context = context;
        _notificationService = notificationService;
        _auditService = auditService;
    }

    public async Task<WorkflowInstance> StartWorkflowAsync(int documentId, int workflowTemplateId, string initiatedById)
    {
        var template = await _context.WorkflowTemplates
            .Include(t => t.Steps)
            .FirstOrDefaultAsync(t => t.Id == workflowTemplateId);

        if (template == null)
        {
            throw new InvalidOperationException("Workflow template not found.");
        }

        var instance = new WorkflowInstance
        {
            WorkflowTemplateId = workflowTemplateId,
            DocumentId = documentId,
            InitiatedById = initiatedById,
            Status = WorkflowStatus.InProgress,
            InitiatedAt = DateTime.UtcNow
        };

        _context.WorkflowInstances.Add(instance);
        await _context.SaveChangesAsync();

        // Create step instances
        foreach (var step in template.Steps.OrderBy(s => s.StepOrder))
        {
            var stepInstance = new WorkflowStepInstance
            {
                WorkflowInstanceId = instance.Id,
                WorkflowStepId = step.Id,
                Status = step.StepOrder == 1 ? StepStatus.InProgress : StepStatus.Pending,
                AssignedToUserId = step.AssignedToUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkflowStepInstances.Add(stepInstance);
        }

        await _context.SaveChangesAsync();

        // Update document status
        var document = await _context.Documents.FindAsync(documentId);
        if (document != null)
        {
            document.Status = DocumentStatus.PendingReview;
            await _context.SaveChangesAsync();
        }

        await _auditService.LogAsync(
            initiatedById,
            "WorkflowStarted",
            "WorkflowInstance",
            instance.Id,
            $"Workflow '{template.Name}' started for document"
        );

        return instance;
    }

    public async Task<IEnumerable<WorkflowInstance>> GetDocumentWorkflowsAsync(int documentId)
    {
        return await _context.WorkflowInstances
            .Include(w => w.WorkflowTemplate)
            .Include(w => w.StepInstances)
                .ThenInclude(s => s.WorkflowStep)
            .Where(w => w.DocumentId == documentId)
            .OrderByDescending(w => w.InitiatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkflowStepInstance>> GetPendingTasksAsync(string userId)
    {
        return await _context.WorkflowStepInstances
            .Include(s => s.WorkflowInstance)
                .ThenInclude(w => w.Document)
            .Include(s => s.WorkflowStep)
            .Where(s => s.AssignedToUserId == userId && s.Status == StepStatus.InProgress)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task ApproveStepAsync(int stepInstanceId, string userId, string? comments = null)
    {
        var stepInstance = await _context.WorkflowStepInstances
            .Include(s => s.WorkflowInstance)
                .ThenInclude(w => w.Document)
            .Include(s => s.WorkflowInstance)
                .ThenInclude(w => w.StepInstances)
            .Include(s => s.WorkflowStep)
            .FirstOrDefaultAsync(s => s.Id == stepInstanceId);

        if (stepInstance == null)
        {
            throw new InvalidOperationException("Workflow step not found.");
        }

        stepInstance.Status = StepStatus.Approved;
        stepInstance.Comments = comments;
        stepInstance.CompletedAt = DateTime.UtcNow;

        // Check if there are more steps
        var nextStep = stepInstance.WorkflowInstance.StepInstances
            .OrderBy(s => s.WorkflowStep.StepOrder)
            .FirstOrDefault(s => s.Status == StepStatus.Pending);

        if (nextStep != null)
        {
            nextStep.Status = StepStatus.InProgress;
        }
        else
        {
            // All steps completed - approve workflow
            stepInstance.WorkflowInstance.Status = WorkflowStatus.Approved;
            stepInstance.WorkflowInstance.CompletedAt = DateTime.UtcNow;

            if (stepInstance.WorkflowInstance.Document != null)
            {
                stepInstance.WorkflowInstance.Document.Status = DocumentStatus.Approved;
            }
        }

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            userId,
            "WorkflowStepApproved",
            "WorkflowStepInstance",
            stepInstanceId,
            $"Workflow step approved"
        );

        // Notify document owner
        if (stepInstance.WorkflowInstance.Document != null)
        {
            await _notificationService.CreateNotificationAsync(
                stepInstance.WorkflowInstance.Document.UploadedById,
                "Workflow Step Approved",
                $"A workflow step for '{stepInstance.WorkflowInstance.Document.Title}' was approved.",
                NotificationType.WorkflowApproved,
                stepInstance.WorkflowInstance.DocumentId,
                "Document"
            );
        }
    }

    public async Task RejectStepAsync(int stepInstanceId, string userId, string? comments = null)
    {
        var stepInstance = await _context.WorkflowStepInstances
            .Include(s => s.WorkflowInstance)
                .ThenInclude(w => w.Document)
            .FirstOrDefaultAsync(s => s.Id == stepInstanceId);

        if (stepInstance == null)
        {
            throw new InvalidOperationException("Workflow step not found.");
        }

        stepInstance.Status = StepStatus.Rejected;
        stepInstance.Comments = comments;
        stepInstance.CompletedAt = DateTime.UtcNow;

        // Reject entire workflow
        stepInstance.WorkflowInstance.Status = WorkflowStatus.Rejected;
        stepInstance.WorkflowInstance.CompletedAt = DateTime.UtcNow;

        if (stepInstance.WorkflowInstance.Document != null)
        {
            stepInstance.WorkflowInstance.Document.Status = DocumentStatus.Rejected;
        }

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            userId,
            "WorkflowStepRejected",
            "WorkflowStepInstance",
            stepInstanceId,
            $"Workflow step rejected: {comments}"
        );

        // Notify document owner
        if (stepInstance.WorkflowInstance.Document != null)
        {
            await _notificationService.CreateNotificationAsync(
                stepInstance.WorkflowInstance.Document.UploadedById,
                "Workflow Rejected",
                $"The workflow for '{stepInstance.WorkflowInstance.Document.Title}' was rejected. Reason: {comments}",
                NotificationType.WorkflowRejected,
                stepInstance.WorkflowInstance.DocumentId,
                "Document"
            );
        }
    }

    public async Task<IEnumerable<WorkflowTemplate>> GetActiveWorkflowTemplatesAsync()
    {
        return await _context.WorkflowTemplates
            .Include(t => t.Steps)
            .Where(t => t.IsActive)
            .ToListAsync();
    }
}
