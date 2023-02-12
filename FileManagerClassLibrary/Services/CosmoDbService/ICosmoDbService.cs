using FileManagerClassLibrary.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Services.CosmoDbService
{
    public interface ICosmosDbService
    {
        Metadata MapData(IFormFile file, string NameInBlobStorage, string description = null);
        Task<IEnumerable<Metadata>> GetAsync(string query);
        Task<Metadata> GetByIdAsync(string Id);
        Task AddAsync(Metadata Item);
    }
}
