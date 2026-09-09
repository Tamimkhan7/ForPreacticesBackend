namespace ForPractices.Service.FileUpload
{
    public class FileUploadService
    {
        Task<string> UploadFIleAsync(IFormFile file);
        void DeleteFile(string? fileUrl);
    }
}
