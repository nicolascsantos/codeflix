using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.UploadMedias
{
    [Collection(nameof(UploadMediasTestFixture))]
    public class UploadMediasTest
    {
        private readonly UploadMediasTestFixture _fixture;
        private readonly UseCases.UploadMedias _useCase;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IStorageService> _storageServiceMock;

        public UploadMediasTest(UploadMediasTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _storageServiceMock = new Mock<IStorageService>();
            _useCase = new UseCases.UploadMedias(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _storageServiceMock.Object
            );
        }

        [Fact(DisplayName = nameof(UploadMedias))]
        [Trait("Application", "UploadMedias - Use Cases")]
        public async Task UploadMedias()
        {
            var videoExample = _fixture.GetValidVideo();

            _videoRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(videoExample);

            _storageServiceMock.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(Guid.NewGuid().ToString());

            var output = await _useCase.Handle(_fixture.GetValidImageFileInput(), CancellationToken.None);

            output.Should().NotBeNull();

            _videoRepositoryMock.VerifyAll();
            _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
                , Times.Exactly(2)
            );
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        }
    }
}
