using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerClassLibrary.Interfaces
{
    public interface IUnitOfWork
    {
        public IFileMetadataRepository FileMetadata { get; }
        public IBlobStorageRepository BlobStorage { get; }

    }
}
