using FileManagerClassLibrary.Interfaces;
using FileManagerClassLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Repositories
{
    public class FileMetadataRepository : IFileMetadataRepository
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public FileMetadataRepository(
                                CosmosClient dbClient,
                                string databaseName,
                                string containerName, IHttpContextAccessor httpContextAccesor)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
            _httpContextAccesor = httpContextAccesor;
        }

        private string Getsizeinfo(long size)
        {
            var sizeinfo = "";
            long fileSizeibBytes = size;
            long fileSizeibKbs = fileSizeibBytes / 1024;
            long fileSizeibMbs = fileSizeibBytes / (1024 * 1024);

            sizeinfo = (fileSizeibMbs > 0) ? (fileSizeibMbs.ToString() + " Mbs") : (fileSizeibKbs > 0) ? (fileSizeibKbs.ToString() + " Kbs") : (fileSizeibBytes + " KiB");
            return sizeinfo;
        }

        public FileMetadata MapData(IFormFile file, string NameInBlobStorage, string description = null)
        {
            FileMetadata metadata = new FileMetadata();
            metadata.Id = Guid.NewGuid().ToString();
            metadata.Description = description;
            metadata.FileName = file.FileName;
            metadata.FileSize = Getsizeinfo(file.Length);
            metadata.ContentType = file.ContentType;
            metadata.UploadedDate = DateTime.Now;
            metadata.NameInBlobStorage = NameInBlobStorage;
            metadata.owner = _httpContextAccesor.HttpContext.User.Identity.Name;
            return metadata;
        }
        public async Task AddAsync(FileMetadata item)
        {
            await _container.CreateItemAsync<FileMetadata>(item, new PartitionKey(item.Id.ToString()));
        }

        public async Task<FileMetadata> GetByIdAsync(string id)
        {
            try
            {
                ItemResponse<FileMetadata> response = await _container.ReadItemAsync<FileMetadata>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<FileMetadata>> GetAllFileMetadaAsync()
        {            
            var query = _container.GetItemQueryIterator<FileMetadata>(new QueryDefinition($"Select * From C where C.owner = @owner").WithParameter("@owner", _httpContextAccesor.HttpContext.User.Identity.Name));
            List<FileMetadata> results = new List<FileMetadata>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<FileMetadata>(id, new PartitionKey(id));
        }
    }
}
