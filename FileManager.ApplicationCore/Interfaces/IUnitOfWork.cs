namespace FileManager.ApplicationCore.Interfaces
{
    public interface IUnitOfWork
    {
        public IFileMetadataRepository FileMetadata { get; }
        public IBlobStorageRepository BlobStorage { get; }

    }
}
