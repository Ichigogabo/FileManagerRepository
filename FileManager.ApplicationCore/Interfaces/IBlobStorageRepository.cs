using FileManager.ApplicationCore.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FileManager.ApplicationCore.Interfaces
{
    public interface IBlobStorageRepository
    {
        Task<CreateFileResponseViewModel> UploadAsync(IFormFile file);
        Task<ReadFileViewModel> DownloadAsync(string blobFilename);
        Task<bool> DeleteAsync(string blobFilename);
    }
}
