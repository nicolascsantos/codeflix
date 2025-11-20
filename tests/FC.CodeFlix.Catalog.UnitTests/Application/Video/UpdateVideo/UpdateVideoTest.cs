using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FluentAssertions;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.UpdateVideo
{
    [Collection(nameof(UpdateVideoTestFixture))]
    public class UpdateVideoTest
    {
        private readonly UpdateVideoTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UseCases.UpdateVideo _useCase;

        public UpdateVideoTest(UpdateVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _useCase = new UseCases.UpdateVideo(
                _videoRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact(DisplayName = nameof(UpdateVideosBasicInfo))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosBasicInfo()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.GetValidInput(exampleVideo.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            var output = await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<DomainEntity.Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Rating == input.Rating)),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));

            output.Should().NotBeNull();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
        }
    }
}
