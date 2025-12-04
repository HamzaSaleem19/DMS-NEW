using DMS.Models;

namespace DMS.Services;

public interface IDocumentService
{
    Task<IEnumerable<Document>> GetAllDocumentsAsync(bool includeDeleted = false);
    Task<IEnumerable<Document>> GetDocumentsByUserAsync(string userId);
    Task<IEnumerable<Document>> GetDocumentsByCategoryAsync(int categoryId);
    Task<Document?> GetDocumentByIdAsync(int id);
    Task<Document> CreateDocumentAsync(Document document);
    Task UpdateDocumentAsync(Document document);
    Task DeleteDocumentAsync(int id, bool softDelete = true);
    Task<bool> CanUserAccessDocument(int documentId, string userId, PermissionType requiredPermission);
    Task CheckOutDocumentAsync(int documentId, string userId);
    Task CheckInDocumentAsync(int documentId, string userId);
    Task<IEnumerable<Document>> GetRecentDocumentsAsync(int count = 10);
    Task<Dictionary<string, int>> GetDocumentStatisticsAsync();
}
