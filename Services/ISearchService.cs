using DMS.Models;

namespace DMS.Services;

public interface ISearchService
{
    Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm, int? categoryId = null, List<int>? tagIds = null, DocumentStatus? status = null);
    Task<IEnumerable<Document>> GetDocumentsByTagAsync(int tagId);
    Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
    Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm);
}
