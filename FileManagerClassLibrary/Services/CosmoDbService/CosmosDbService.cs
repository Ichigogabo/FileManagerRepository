using FileManagerClassLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Services.CosmoDbService
{
    public class CosmosDbService : ICosmosDbService
    {

        private readonly Microsoft.Azure.Cosmos.Container _container;

        public CosmosDbService(
                                CosmosClient dbClient,
                                string databaseName,
                                string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public Metadata MapData(IFormFile file, string NameInBlobStorage, string description = null)
        {
            Metadata metadata = new Metadata();
            metadata.Id = Guid.NewGuid().ToString();
            metadata.Description = description;
            metadata.FileName = file.FileName;
            metadata.FileSize = file.Length;
            metadata.ContentType = file.ContentType;
            metadata.UploadedDate = DateTime.Now;
            metadata.NameInBlobStorage = NameInBlobStorage;
            return metadata;
        }
        public async Task AddAsync(Metadata item)
        {
            await this._container.CreateItemAsync<Metadata>(item, new PartitionKey(item.Id.ToString()));
        }

        public async Task<Metadata> GetByIdAsync(string id)
        {
            try
            {
                ItemResponse<Metadata> response = await this._container.ReadItemAsync<Metadata>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Metadata>> GetAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Metadata>(new QueryDefinition(queryString));
            List<Metadata> results = new List<Metadata>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

    }
}
