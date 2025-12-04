using Microsoft.AspNetCore.Components.Forms;

namespace DMS.Services;

public interface IFileStorageService
{
    Task<(string filePath, long fileSize)> SaveFileAsync(IBrowserFile file, string fileName);
    Task<byte[]> GetFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
    Task<string> GetFileContentTypeAsync(string fileName);
    bool IsValidFileExtension(string fileName);
    long GetMaxFileSize();
}
