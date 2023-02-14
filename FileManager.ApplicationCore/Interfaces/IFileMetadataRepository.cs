using FileManager.ApplicationCore.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileManager.ApplicationCore.Interfaces
{
    public interface IFileMetadataRepository
    {
        public FileMetadata MapData(IFormFile file, string userName, string nameInBlobStorage, string description = null);
        public Task<IEnumerable<FileMetadata>> GetAllFileMetadaAsync(string userName);
        public Task<FileMetadata> GetByIdAsync(string id);
        public Task AddAsync(FileMetadata item);
        public Task DeleteAsync(string id);

    }
}
