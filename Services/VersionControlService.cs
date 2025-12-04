using Microsoft.EntityFrameworkCore;
using DMS.Data;
using DMS.Models;

namespace DMS.Services;

public class VersionControlService : IVersionControlService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public VersionControlService(ApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<DocumentVersion> CreateVersionAsync(int documentId, string filePath, long fileSize, string changeDescription, string userId)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null)
        {
            throw new InvalidOperationException("Document not found.");
        }

        // Mark previous versions as not current
        var previousVersions = await _context.DocumentVersions
            .Where(v => v.DocumentId == documentId && v.IsCurrent)
            .ToListAsync();

        foreach (var version in previousVersions)
        {
            version.IsCurrent = false;
        }

        // Create new version
        var newVersion = new DocumentVersion
        {
            DocumentId = documentId,
            VersionNumber = document.CurrentVersion + 1,
            FilePath = filePath,
            FileSize = fileSize,
            ChangeDescription = changeDescription,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
            IsCurrent = true
        };

        _context.DocumentVersions.Add(newVersion);

        // Update document version number
        document.CurrentVersion = newVersion.VersionNumber;
        document.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            userId,
            "VersionCreated",
            "DocumentVersion",
            newVersion.Id,
            $"Version {newVersion.VersionNumber} created for document '{document.Title}'"
        );

        return newVersion;
    }

    public async Task<IEnumerable<DocumentVersion>> GetDocumentVersionsAsync(int documentId)
    {
        return await _context.DocumentVersions
            .Include(v => v.CreatedBy)
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async Task<DocumentVersion?> GetVersionByIdAsync(int versionId)
    {
        return await _context.DocumentVersions
            .Include(v => v.Document)
            .Include(v => v.CreatedBy)
            .FirstOrDefaultAsync(v => v.Id == versionId);
    }

    public async Task<DocumentVersion?> GetCurrentVersionAsync(int documentId)
    {
        return await _context.DocumentVersions
            .Include(v => v.CreatedBy)
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.IsCurrent);
    }

    public async Task RestoreVersionAsync(int versionId, string userId)
    {
        var version = await GetVersionByIdAsync(versionId);
        if (version == null)
        {
            throw new InvalidOperationException("Version not found.");
        }

        // Mark all versions as not current
        var allVersions = await _context.DocumentVersions
            .Where(v => v.DocumentId == version.DocumentId)
            .ToListAsync();

        foreach (var v in allVersions)
        {
            v.IsCurrent = false;
        }

        // Mark restored version as current
        version.IsCurrent = true;

        // Update document
        var document = await _context.Documents.FindAsync(version.DocumentId);
        if (document != null)
        {
            document.CurrentVersion = version.VersionNumber;
            document.ModifiedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            userId,
            "VersionRestored",
            "DocumentVersion",
            versionId,
            $"Version {version.VersionNumber} restored for document"
        );
    }
}
