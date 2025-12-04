using Microsoft.EntityFrameworkCore;
using DMS.Data;
using DMS.Models;

namespace DMS.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public DocumentService(ApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<IEnumerable<Document>> GetAllDocumentsAsync(bool includeDeleted = false)
    {
        var query = _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
            .AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(d => !d.IsDeleted);
        }

        return await query.OrderByDescending(d => d.UploadedAt).ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByUserAsync(string userId)
    {
        return await _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
            .Where(d => d.UploadedById == userId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByCategoryAsync(int categoryId)
    {
        return await _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
            .Where(d => d.CategoryId == categoryId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<Document?> GetDocumentByIdAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Include(d => d.DocumentTags)
                .ThenInclude(dt => dt.Tag)
            .Include(d => d.Versions)
            .Include(d => d.Permissions)
            .Include(d => d.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Document> CreateDocumentAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            document.UploadedById,
            "DocumentCreated",
            "Document",
            document.Id,
            $"Document '{document.Title}' was created"
        );

        return document;
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        document.ModifiedAt = DateTime.UtcNow;
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            document.UploadedById,
            "DocumentUpdated",
            "Document",
            document.Id,
            $"Document '{document.Title}' was updated"
        );
    }

    public async Task DeleteDocumentAsync(int id, bool softDelete = true)
    {
        var document = await GetDocumentByIdAsync(id);
        if (document == null) return;

        if (softDelete)
        {
            document.IsDeleted = true;
            document.DeletedAt = DateTime.UtcNow;
            _context.Documents.Update(document);
        }
        else
        {
            _context.Documents.Remove(document);
        }

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            document.UploadedById,
            "DocumentDeleted",
            "Document",
            document.Id,
            $"Document '{document.Title}' was deleted"
        );
    }

    public async Task<bool> CanUserAccessDocument(int documentId, string userId, PermissionType requiredPermission)
    {
        var document = await _context.Documents
            .Include(d => d.Permissions)
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (document == null) return false;

        // Document owner always has full access
        if (document.UploadedById == userId) return true;

        // Check explicit permissions
        var permission = await _context.DocumentPermissions
            .FirstOrDefaultAsync(p => p.DocumentId == documentId && p.UserId == userId);

        if (permission != null)
        {
            return permission.PermissionType >= requiredPermission;
        }

        return false;
    }

    public async Task CheckOutDocumentAsync(int documentId, string userId)
    {
        var document = await GetDocumentByIdAsync(documentId);
        if (document == null) return;

        document.CheckedOutBy = userId;
        document.CheckedOutAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            userId,
            "DocumentCheckedOut",
            "Document",
            documentId,
            $"Document '{document.Title}' was checked out"
        );
    }

    public async Task CheckInDocumentAsync(int documentId, string userId)
    {
        var document = await GetDocumentByIdAsync(documentId);
        if (document == null || document.CheckedOutBy != userId) return;

        document.CheckedOutBy = null;
        document.CheckedOutAt = null;
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            userId,
            "DocumentCheckedIn",
            "Document",
            documentId,
            $"Document '{document.Title}' was checked in"
        );
    }

    public async Task<IEnumerable<Document>> GetRecentDocumentsAsync(int count = 10)
    {
        return await _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Where(d => !d.IsDeleted)
            .OrderByDescending(d => d.UploadedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetDocumentStatisticsAsync()
    {
        var stats = new Dictionary<string, int>
        {
            ["TotalDocuments"] = await _context.Documents.CountAsync(d => !d.IsDeleted),
            ["DraftDocuments"] = await _context.Documents.CountAsync(d => !d.IsDeleted && d.Status == DocumentStatus.Draft),
            ["ApprovedDocuments"] = await _context.Documents.CountAsync(d => !d.IsDeleted && d.Status == DocumentStatus.Approved),
            ["PendingReview"] = await _context.Documents.CountAsync(d => !d.IsDeleted && d.Status == DocumentStatus.PendingReview),
            ["TotalCategories"] = await _context.Categories.CountAsync(c => c.IsActive),
            ["TotalTags"] = await _context.Tags.CountAsync()
        };

        return stats;
    }
}
