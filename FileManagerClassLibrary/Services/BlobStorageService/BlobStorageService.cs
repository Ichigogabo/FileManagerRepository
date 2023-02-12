using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Services.BlobStorageService
{
    public class BlobStorageService : IBlobStorageService
    {

        private readonly string _connectionString;
        private readonly string _blobContainerName;
         
        public BlobStorageService(IConfiguration configuration)
        {
            this._connectionString = configuration.GetValue<string>("ConnectionString");
            this._blobContainerName = configuration.GetValue<string>("BlobContainerName");
        }

        public async Task<string> UploadAsync(IFormFile file, string blobName = null)
        {
            if (file.Length == 0) return null;

            BlobContainerClient blobContainerClient = new BlobContainerClient(this._connectionString, _blobContainerName);

            blobName = Guid.NewGuid().ToString();
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType };

            await using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
            }

            return blobName;
        }

    }
}
