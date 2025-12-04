using DMS.Models;

namespace DMS.Services;

public interface IWorkflowService
{
    Task<WorkflowInstance> StartWorkflowAsync(int documentId, int workflowTemplateId, string initiatedById);
    Task<IEnumerable<WorkflowInstance>> GetDocumentWorkflowsAsync(int documentId);
    Task<IEnumerable<WorkflowStepInstance>> GetPendingTasksAsync(string userId);
    Task ApproveStepAsync(int stepInstanceId, string userId, string? comments = null);
    Task RejectStepAsync(int stepInstanceId, string userId, string? comments = null);
    Task<IEnumerable<WorkflowTemplate>> GetActiveWorkflowTemplatesAsync();
}
