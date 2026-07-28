using Amazon.S3;
using Amazon.S3.Model;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Infra.Storage.Configuration;
using Microsoft.Extensions.Options;

namespace FC.CodeFlix.Catalog.Infra.Storage.Services
{
    public class AWSStorageService : IStorageService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly StorageServiceOptions _storageServiceOptions;

        public AWSStorageService(IAmazonS3 amazonS3, IOptions<StorageServiceOptions> storageServiceOptions)
        {
            _amazonS3 = amazonS3;
            _storageServiceOptions = storageServiceOptions.Value;
        }

        public async Task Delete(string filePath, CancellationToken cancellationToken)
            => await _amazonS3.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _storageServiceOptions.BucketName,
                Key = filePath
            }, cancellationToken);

        public async Task<string> Upload(string fileName, Stream fileStream, string contentType, CancellationToken cancellationToken)
        {
            await _amazonS3.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _storageServiceOptions.BucketName,
                Key = fileName,
                InputStream = fileStream,
                ContentType = contentType,
                AutoCloseStream = false
            }, cancellationToken);

            return fileName;
        }
    }
}
