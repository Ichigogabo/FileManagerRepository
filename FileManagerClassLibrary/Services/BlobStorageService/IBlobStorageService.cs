using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Services.BlobStorageService
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(IFormFile file, string blobname = null);

    }
}
