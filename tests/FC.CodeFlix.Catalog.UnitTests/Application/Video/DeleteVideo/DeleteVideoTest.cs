using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
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


        public DeleteVideoTest(DeleteVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _useCase = new UseCases.DeleteVideo(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object
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
    }
}
