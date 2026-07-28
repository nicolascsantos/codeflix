using Amazon.S3;
using Amazon.S3.Model;
using FC.CodeFlix.Catalog.Infra.Storage.Configuration;
using FC.CodeFlix.Catalog.Infra.Storage.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;

namespace FC.CodeFlix.Catalog.UnitTests.Infra.Storage
{
    [Collection(nameof(StorageServiceTestFixture))]
    public class AWSStorageServiceTest
    {
        private readonly StorageServiceTestFixture _fixture;

        public AWSStorageServiceTest(StorageServiceTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Upload))]
        [Trait("Infra.Storage", "AWSStorageService")]
        public async Task Upload()
        {
            var storageServiceOptions = new StorageServiceOptions(_fixture.GetBucketName());
            var amazonS3ClientMock = new Mock<IAmazonS3>();
            var outputObject = new PutObjectResponse();
            var options = Options.Create(storageServiceOptions);


            amazonS3ClientMock.Setup(x =>
                x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputObject);

            var service = new AWSStorageService(amazonS3ClientMock.Object, options);

            var fileName = _fixture.GetFileName();
            var contentStream = Encoding.UTF8.GetBytes(_fixture.GetFileContent());
            var stream = new MemoryStream(contentStream);
            var contentType = _fixture.GetContentType();

            var filePath = await service.Upload(
                fileName,
                stream,
                contentType,
                CancellationToken.None
            );

            Assert.Equal(fileName, filePath);
            amazonS3ClientMock.Verify(x =>
                x.PutObjectAsync(
                    It.Is<PutObjectRequest>(r => r.BucketName == storageServiceOptions.BucketName &&
                    r.Key == fileName &&
                    r.ContentType == contentType &&
                    r.InputStream == stream), It.IsAny<CancellationToken>())
                , Times.Once);
        }
    }
}
