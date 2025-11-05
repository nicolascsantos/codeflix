using FC.CodeFlix.Catalog.Application.Interfaces;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.CreateVideo
{
    [Collection(nameof(CreateVideoTestFixture))]
    public class CreateVideoTest
    {
        private readonly CreateVideoTestFixture _fixture;

        public CreateVideoTest(CreateVideoTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateVideo))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideo()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = UseCases.CreateVideo();
            (
                repositoryMock.Object,
                unitOfWorkMock.Object
            );

            var input = new CreateVideoInput
            (
                _fixture.GetValidTitle(),
                _fixture.GetValidDescription(),
                _fixture.GetValidYearLaunched(),
                _fixture.GetRandomBoolean(),
                _fixture.GetRandomBoolean(),
                _fixture.GetValidDuration(),
                _fixture.GetRandomRating()
            );



            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(
                x => x.Insert(It.IsAny<DomainEntity.Video>(
                    video => video.Title == input.Title &&
                    video.Description == input.Description &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened &&
                    video.Published == input.Published &&
                    video.Duration == input.Duration &&
                    video.Rating == input.Rating &&
                    video.Id != Guid.Empty
            ), It.IsAny<CancellationToken>()), Times.Once);

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating);
            output.CreatedAt.Should().NotBeNull();
        }
    }
}
