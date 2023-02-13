using FileManagerClassLibrary.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Interfaces
{
    public interface IBlobStorageRepository
    {
        //Task<string> UploadAsync(IFormFile file, string blobname = null);

        Task<CreateFileResponseViewModel> UploadAsync(IFormFile file);

        Task<ReadFileViewModel> DownloadAsync(string blobFilename);
    }
}
