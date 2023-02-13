using FileManagerClassLibrary.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Interfaces
{
    public interface IFileMetadataRepository
    {
        public FileMetadata MapData(IFormFile file, string NameInBlobStorage, string description = null);
        public Task<IEnumerable<FileMetadata>> GetAllFileMetadaAsync();
        public Task<FileMetadata> GetByIdAsync(string Id);
        public Task AddAsync(FileMetadata Item);

    }
}
