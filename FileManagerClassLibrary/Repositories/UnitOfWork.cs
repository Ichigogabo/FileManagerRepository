using FileManagerClassLibrary.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly ILogger<UnitOfWork> _logger;

        private IFileMetadataRepository _fileMetadata;
        private IBlobStorageRepository _blobStorage;

        public UnitOfWork(IConfiguration configuration, IHttpContextAccessor httpContextAccesor, ILogger<UnitOfWork> logger)
        {
            _configuration = configuration;
            _httpContextAccesor = httpContextAccesor;
            _logger = logger;
        }
        public IFileMetadataRepository FileMetadata 
        {
            get
            {  
               return _fileMetadata != null ? _fileMetadata : _fileMetadata = InitializerCosmoClientInstanceAsync(_configuration).GetAwaiter().GetResult();
            }
        }

    public IBlobStorageRepository BlobStorage
        {
            get
            {
                return _blobStorage != null ? _blobStorage : _blobStorage = new BlobStorageRepository(_configuration,_logger);
            }
        }
      

        private async Task<FileMetadataRepository> InitializerCosmoClientInstanceAsync(IConfiguration configuration)
        {
            IConfigurationSection configurationSection = configuration.GetSection("CosmoDb");
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string Key = configurationSection.GetSection("Key").Value;

            Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder clientBuilder = new Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder(account, Key);
            Microsoft.Azure.Cosmos.CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            FileMetadataRepository fileMetadataRepository = new FileMetadataRepository(client, databaseName, containerName, _httpContextAccesor);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return fileMetadataRepository;

        }



    }
}
