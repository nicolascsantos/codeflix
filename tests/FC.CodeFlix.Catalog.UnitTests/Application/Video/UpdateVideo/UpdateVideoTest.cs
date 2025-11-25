using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FluentAssertions;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.CodeFlix.Catalog.Domain.Exceptions;

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

        [Theory(DisplayName = nameof(UpdateVideosThrowsWhenRecieveInvalidInput))]
        [ClassData(typeof(UpdateVideoTestDataGenerator))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosThrowsWhenRecieveInvalidInput(
            UpdateVideoInput invalidInput,
            string expectedExceptionMessage
        )
        {
            var exampleVideo = _fixture.GetValidVideo();

            _videoRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            var action = async () => await _useCase.Handle(invalidInput, CancellationToken.None);

            var exceptionAssertion = await action.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage("There are validation errors.");

            exceptionAssertion.Which.Errors!
                .ToList()[0].Message.Should().Be(expectedExceptionMessage);

            _videoRepositoryMock.VerifyAll();
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = nameof(UpdateVideosThrowsWhenVideoNotFound))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosThrowsWhenVideoNotFound()
        {
            var input = _fixture.GetValidInput(Guid.NewGuid());

            _videoRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new NotFoundException("Video not found."));

            var action = async () => await _useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Video not found.");

            _videoRepositoryMock.Verify(x => x.Update(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
