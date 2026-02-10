namespace FC.CodeFlix.Catalog.Application.Interfaces
{
    public interface IStorageService
    {
        public Task<string> Upload(
            string fileName,
            Stream fileStream,
            string contentType,
            CancellationToken cancellationToken
        );
        public Task Delete(string filePath, CancellationToken cancellationToken);
    }
}
