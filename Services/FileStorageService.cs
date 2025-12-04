using Microsoft.AspNetCore.Components.Forms;

namespace DMS.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly string _rootPath;
    private readonly long _maxFileSize;
    private readonly string[] _allowedExtensions;

    public FileStorageService(IConfiguration configuration)
    {
        _configuration = configuration;
        _rootPath = configuration["FileStorage:RootPath"] ?? "DocumentStorage";
        _maxFileSize = configuration.GetValue<long>("FileStorage:MaxFileSize", 104857600); // 100MB default
        _allowedExtensions = configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
            ?? new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };

        // Ensure root directory exists
        if (!Directory.Exists(_rootPath))
        {
            Directory.CreateDirectory(_rootPath);
        }
    }

    public async Task<(string filePath, long fileSize)> SaveFileAsync(IBrowserFile file, string fileName)
    {
        if (!IsValidFileExtension(fileName))
        {
            throw new InvalidOperationException($"File extension is not allowed.");
        }

        if (file.Size > _maxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {_maxFileSize / 1024 / 1024}MB.");
        }

        // Create year/month directory structure
        var yearMonth = DateTime.UtcNow.ToString("yyyy/MM");
        var directoryPath = Path.Combine(_rootPath, yearMonth);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(directoryPath, uniqueFileName);

        // Save file
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.OpenReadStream(_maxFileSize).CopyToAsync(fileStream);
        }

        return (filePath, file.Size);
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found.", filePath);
        }

        return await File.ReadAllBytesAsync(filePath);
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    public Task<string> GetFileContentTypeAsync(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var contentType = extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            _ => "application/octet-stream"
        };

        return Task.FromResult(contentType);
    }

    public bool IsValidFileExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }

    public long GetMaxFileSize()
    {
        return _maxFileSize;
    }
}
