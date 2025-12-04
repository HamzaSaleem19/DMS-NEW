using Microsoft.EntityFrameworkCore;
using DMS.Data;
using DMS.Models;

namespace DMS.Services;

public class SearchService : ISearchService
{
    private readonly ApplicationDbContext _context;

    public SearchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm, int? categoryId = null, List<int>? tagIds = null, DocumentStatus? status = null)
    {
        var query = _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
            .Where(d => !d.IsDeleted)
            .AsQueryable();

        // Search in title, description, and filename
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(d =>
                d.Title.Contains(searchTerm) ||
                (d.Description != null && d.Description.Contains(searchTerm)) ||
                d.FileName.Contains(searchTerm));
        }

        // Filter by category
        if (categoryId.HasValue)
        {
            query = query.Where(d => d.CategoryId == categoryId.Value);
        }

        // Filter by tags
        if (tagIds != null && tagIds.Any())
        {
            query = query.Where(d => d.DocumentTags.Any(dt => tagIds.Contains(dt.TagId)));
        }

        // Filter by status
        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }

        return await query
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByTagAsync(int tagId)
    {
        return await _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
            .Where(d => !d.IsDeleted && d.DocumentTags.Any(dt => dt.TagId == tagId))
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
    {
        return await _context.Categories
            .Where(c => c.IsActive &&
                (c.Name.Contains(searchTerm) ||
                (c.Description != null && c.Description.Contains(searchTerm))))
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm)
    {
        return await _context.Tags
            .Where(t => t.Name.Contains(searchTerm))
            .ToListAsync();
    }
}
