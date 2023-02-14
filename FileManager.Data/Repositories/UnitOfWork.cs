using System.Threading.Tasks;
using FileManager.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileManager.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UnitOfWork> _logger;

        private IFileMetadataRepository _fileMetadata;
        private IBlobStorageRepository _blobStorage;

        public UnitOfWork(IConfiguration configuration, ILogger<UnitOfWork> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public IFileMetadataRepository FileMetadata => _fileMetadata != null ? _fileMetadata : _fileMetadata = InitializerCosmoClientInstanceAsync(_configuration).GetAwaiter().GetResult();

        public IBlobStorageRepository BlobStorage => _blobStorage != null ? _blobStorage : _blobStorage = new BlobStorageRepository(_configuration,_logger);


        private async Task<FileMetadataRepository> InitializerCosmoClientInstanceAsync(IConfiguration configuration)
        {
            IConfigurationSection configurationSection = configuration.GetSection("CosmoDb");
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string Key = configurationSection.GetSection("Key").Value;

            Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder clientBuilder = new Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder(account, Key);
            Microsoft.Azure.Cosmos.CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            FileMetadataRepository fileMetadataRepository = new FileMetadataRepository(client, databaseName, containerName);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return fileMetadataRepository;

        }
    }
}
