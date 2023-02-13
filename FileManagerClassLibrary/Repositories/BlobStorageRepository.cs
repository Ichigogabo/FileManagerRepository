using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileManagerClassLibrary.Interfaces;
using FileManagerClassLibrary.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Repositories
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly string _connectionString;
        private readonly string _blobContainerName;
        private readonly ILogger _logger;

        public BlobStorageRepository(IConfiguration configuration, ILogger logger)
        {
            IConfigurationSection configurationSection = configuration.GetSection("BlobStorage");
            _connectionString = configurationSection.GetSection("ConnectionString").Value;
            _blobContainerName = configurationSection.GetSection("BlobContainerName").Value;
            _logger = logger;
        }

        public async Task<CreateFileResponseViewModel> UploadAsync(IFormFile blob)
        {

            var response = new CreateFileResponseViewModel();
            var filename = $"{ DateTime.Now.ToString("yyyyMMddHHmmssfff") }_{ blob.FileName }";

            BlobContainerClient container = new BlobContainerClient(_connectionString, _blobContainerName);            
            try
            {
                BlobClient client = container.GetBlobClient(filename);

                await using (Stream? data = blob.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }

                response.FileUri = client.Uri.AbsoluteUri;
                response.FileName = client.Name;
                response.Success = true;
            }          
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            }

            return response;
        }

        public async Task<ReadFileViewModel> DownloadAsync(string blobFilename)
        {
            BlobContainerClient client = new BlobContainerClient(_connectionString, _blobContainerName);

            try
            {
                BlobClient file = client.GetBlobClient(blobFilename);

                if (await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;

                    var content = await file.DownloadContentAsync();

                    string name = blobFilename;
                    string contentType = content.Value.Details.ContentType;

                   return new ReadFileViewModel { Content = blobContent, Name = name, ContentType = contentType };
                }
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                
            }

            // File does not exist, return null and handle that in requesting method
            return null;
        }

        public async Task<bool> DeleteAsync(string blobFilename)
        {
            BlobContainerClient client = new BlobContainerClient(_connectionString, _blobContainerName);

            BlobClient file = client.GetBlobClient(blobFilename);

            try
            {
               await file.DeleteAsync();
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
               _logger.LogError($"File {blobFilename} was not found.");
                return false;
            }

           return true;

        }
    }
}
