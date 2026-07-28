using FC.CodeFlix.Catalog.Application.Interfaces;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base
{
    public class FakeStorageService : IStorageService
    {
        public Task<string> Upload(
            string fileName,
            Stream fileStream,
            string contentType,
            CancellationToken cancellationToken
        ) => Task.FromResult(fileName);

        public Task Delete(string filePath, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
