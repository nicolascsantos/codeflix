using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.DeleteVideo;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.DeleteVideo
{
    [Collection(nameof(DeleteVideoTestFixture))]
    public class DeleteVideoTest
    {
        private readonly DeleteVideoTestFixture _fixture;
        private readonly UseCases.DeleteVideo _useCase;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IStorageService> _storageService;


        public DeleteVideoTest(DeleteVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _storageService = new Mock<IStorageService>();
            _useCase = new UseCases.DeleteVideo(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _storageService.Object
            );
        }

        [Fact(DisplayName = nameof(DeleteVideo))]
        [Trait("Application", "DeleteVideo - Use Cases")]
        public async Task DeleteVideo()
        {
            var videoExample = _fixture.GetValidVideo();
            var input = _fixture.GetValidDeleteVideoInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = nameof(DeleteVideoWithAllMediasAndCleanStorage))]
        [Trait("Application", "DeleteVideo - Use Cases")]
        public async Task DeleteVideoWithAllMediasAndCleanStorage()
        {
            var videoExample = _fixture.GetValidVideo();
            videoExample.UpdateMedia(_fixture.GetValidMediaPath());
            videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
            var filePaths = new List<string>() { videoExample.Media!.FilePath, videoExample.Trailer!.FilePath };
            var input = _fixture.GetValidDeleteVideoInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
            _storageService.Verify(x => x.Delete(
                It.Is<string>(x => x == videoExample.Media!.FilePath),
                It.IsAny<CancellationToken>()
            ));
            _storageService.Verify(x => x.Delete(
                It.Is<string>(filePath => filePaths.Contains(filePath)),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
            _storageService.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
        }

        [Fact(DisplayName = nameof(DeleteVideoWithOnlyTrailerAndCleanStorageOnlyForTrailer))]
        [Trait("Application", "DeleteVideo - Use Cases")]
        public async Task DeleteVideoWithOnlyTrailerAndCleanStorageOnlyForTrailer()
        {
            var videoExample = _fixture.GetValidVideo();
            videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
            var input = _fixture.GetValidDeleteVideoInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
            _storageService.Verify(x => x.Delete(
                It.Is<string>(filePath => filePath == videoExample.Trailer!.FilePath),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(1));
            _storageService.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(1));
        }

        [Fact(DisplayName = nameof(DeleteVideoWithOnlyMediaAndCleanStorageOnlyForMedia))]
        [Trait("Application", "DeleteVideo - Use Cases")]
        public async Task DeleteVideoWithOnlyMediaAndCleanStorageOnlyForMedia()
        {
            var videoExample = _fixture.GetValidVideo();
            videoExample.UpdateMedia(_fixture.GetValidMediaPath());
            var input = _fixture.GetValidDeleteVideoInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
            _storageService.Verify(x => x.Delete(
                It.Is<string>(filePath => filePath == videoExample.Media!.FilePath),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(1));
            _storageService.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(1));
        }

        [Fact(DisplayName = nameof(DeleteVideoWithoutAnyMediaAndDontCleanStorage))]
        [Trait("Application", "DeleteVideo - Use Cases")]
        public async Task DeleteVideoWithoutAnyMediaAndDontCleanStorage()
        {
            var videoExample = _fixture.GetValidVideo();
            var input = _fixture.GetValidDeleteVideoInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(videoExample);

            await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _videoRepositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
            _storageService.Verify(x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }

        [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
        [Trait("Application", "DeleteVideo - Use Cases")]
        public async Task ThrowsWhenVideoNotFound()
        {
            var videoExample = _fixture.GetValidVideo();
            var input = _fixture.GetValidDeleteVideoInput(videoExample.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new NotFoundException($"Video '{videoExample.Id}' not found."));

            var action = async () => await _useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Video '{videoExample.Id}' not found.");

            _videoRepositoryMock.VerifyAll();
            _videoRepositoryMock.Verify(x => x.Delete(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Never);
            _storageService.Verify(x => x.Delete(
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }
    }
}
