using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Infra.Storage.Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace FC.CodeFlix.Catalog.Infra.Storage.Services
{
    public class StorageService : IStorageService
    {
        public StorageService(StorageClient storageClient, IOptions<StorageServiceOptions> options)
        {
            
        }

        public Task Delete(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> Upload(string fileName, Stream fileStream, string contentType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
