using FC.CodeFlix.Catalog.Application.Interfaces;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class StorageService : IStorageService
    {
        public Task Delete(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> Upload(string fileName, Stream fileStream, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
