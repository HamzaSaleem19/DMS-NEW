using DMS.Models;

namespace DMS.Services;

public interface IVersionControlService
{
    Task<DocumentVersion> CreateVersionAsync(int documentId, string filePath, long fileSize, string changeDescription, string userId);
    Task<IEnumerable<DocumentVersion>> GetDocumentVersionsAsync(int documentId);
    Task<DocumentVersion?> GetVersionByIdAsync(int versionId);
    Task<DocumentVersion?> GetCurrentVersionAsync(int documentId);
    Task RestoreVersionAsync(int versionId, string userId);
}
