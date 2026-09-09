namespace ForPractices.Service.FileUpload
{
    public interface IFileUploadService
    {
        Task<string> UploadFIleAsync(IFormFile file);
        void DeleteFile(string? fileUrl);
    }
}
