using FluentAssertions;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;
using GcpData = Google.Apis.Storage.v1.Data;

namespace FC.CodeFlix.Catalog.UnitTests.Infra.Storage
{
    public class StorageServiceTest
    {
        [Fact(DisplayName = nameof(Upload))]
        [Trait("Infra.Storage", "StorageService")]
        public async Task Upload()
        {
            var storageClientMock = new Mock<StorageClient>();
            var objectMock = new Mock<GcpData.Object>();
            storageClientMock.Setup(x => x.UploadObjectAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<UploadObjectOptions>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<IProgress<IUploadProgress>>()
            )).ReturnsAsync(objectMock.Object);

            var storageOptions = new StorageServiceOptions
            {
                BucketName = "test"
            };
            var options = Options.Create(storageOptions);

            var service = new StorageService(storageClientMock.Object, options);

            var fileName = "video/test.mp4";
            var contentStream = Encoding.UTF8.GetBytes("content-example");
            var stream = new MemoryStream(contentStream);
            var contentType = "video/mp4";

            var filePath = await service.Upload(
                fileName, 
                stream,
                contentType,
                CancellationToken.None
            );

            Assert.Equal(fileName, filePath);
            storageClientMock.Verify(x => x.UploadObjectAsync(
                storageOptions.BucketName,
                fileName,
                contentType,
                stream,
                It.IsAny<UploadObjectOptions>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<IProgress<IUploadProgress>>()
            ), Times.Once);
        }
    }
}
