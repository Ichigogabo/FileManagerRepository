using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileManagerClassLibrary.Interfaces;
using FileManagerClassLibrary.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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

        public BlobStorageRepository(IConfiguration configuration)
        {
            IConfigurationSection configurationSection = configuration.GetSection("BlobStorage");
            _connectionString = configurationSection.GetSection("ConnectionString").Value;
            _blobContainerName = configurationSection.GetSection("BlobContainerName").Value;
        }
        //public async Task<string> UploadAsync(IFormFile file, string blobName = null)
        //{
        //    if (file.Length == 0) return null;

        //    BlobContainerClient blobContainerClient = new BlobContainerClient(this._connectionString, _blobContainerName);

        //    blobName = Guid.NewGuid().ToString();
        //    var blobClient = blobContainerClient.GetBlobClient(blobName);
        //    BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType };

        //    await using (Stream stream = file.OpenReadStream())
        //    {
        //        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
        //    }

        //    return blobName;
        //}

        public async Task<CreateFileResponseViewModel> UploadAsync(IFormFile blob)
        {

            var response = new CreateFileResponseViewModel();
            var filename = $"{ DateTime.Now.ToString("yyyyMMddHHmmssfff") }_{ blob.FileName }";

            // Get a reference to a container named in appsettings.json and then create it
            BlobContainerClient container = new BlobContainerClient(_connectionString, _blobContainerName);
            //await container.CreateAsync();
            try
            {
                // Get a reference to the blob just uploaded from the API in a container from configuration settings
                BlobClient client = container.GetBlobClient(filename);

                // Open a stream for the file we want to upload
                await using (Stream? data = blob.OpenReadStream())
                {
                    // Upload the file async
                    await client.UploadAsync(data);
                }

                response.FileUri = client.Uri.AbsoluteUri;
                response.FileName = client.Name;



            }
            // If the file already exists, we catch the exception and do not upload it
            catch (RequestFailedException ex)
               when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                
            }
            // If we get an unexpected error, we catch it here and return the error message
            catch (RequestFailedException ex)
            {
               
            }

            // Return the BlobUploadResponse object
            return response;
        }

        public async Task<ReadFileViewModel> DownloadAsync(string blobFilename)
        {
            // Get a reference to a container named in appsettings.json
            BlobContainerClient client = new BlobContainerClient(_connectionString, _blobContainerName);

            try
            {
                // Get a reference to the blob uploaded earlier from the API in the container from configuration settings
                BlobClient file = client.GetBlobClient(blobFilename);

                // Check if the file exists in the container
                if (await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;

                    // Download the file details async
                    var content = await file.DownloadContentAsync();

                    // Add data to variables in order to return a BlobDto
                    string name = blobFilename;
                    string contentType = content.Value.Details.ContentType;

                    // Create new BlobDto with blob data from variables
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
    }
}
