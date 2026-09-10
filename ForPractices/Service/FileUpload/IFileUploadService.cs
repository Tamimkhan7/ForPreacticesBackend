namespace ForPractices.Service.FileUpload
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string subFolder);
        void DeleteFile(string? fileUrl);
    }
}