namespace FC.CodeFlix.Catalog.Application.Interfaces
{
    public interface IStorageService
    {
        public Task<string> Upload(string fileName, Stream fileStream, CancellationToken cancellationToken);
    }
}
