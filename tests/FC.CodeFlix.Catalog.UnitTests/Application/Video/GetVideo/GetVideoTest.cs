using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.GetVideo;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.GetVideo
{
    [Collection(nameof(GetVideoTestFixture))]
    public class GetVideoTest
    {
        private readonly GetVideoTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly UseCases.GetVideo _useCase;

        public GetVideoTest(GetVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _useCase = new UseCases.GetVideo(_videoRepositoryMock.Object);
        }

        [Fact(DisplayName = nameof(GetVideo))]
        [Trait("Application", "GetVideo - Use Cases")]
        public async Task GetVideo()
        {
            var videoExample = _fixture.GetValidVideo();

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(videoExample);

            var input = new UseCases.GetVideoInput(videoExample.Id);
            var output = await _useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(videoExample.Id);
            output.Title.Should().Be(videoExample.Title);
            output.Description.Should().Be(videoExample.Description);
            output.YearLaunched.Should().Be(videoExample.YearLaunched);
            output.Opened.Should().Be(videoExample.Opened);
            output.Published.Should().Be(videoExample.Published);
            output.Duration.Should().Be(videoExample.Duration);
            output.CreatedAt.Should().Be(videoExample.CreatedAt);
            output.Rating.Should().Be(videoExample.Rating.ToStringSignal());
            _videoRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
        [Trait("Application", "GetVideo - Use Cases")]
        public async Task ThrowsWhenVideoNotFound()
        {
            var videoExample = _fixture.GetValidVideo();

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new NotFoundException($"Video '{videoExample.Id}' not found."));

            var input = new UseCases.GetVideoInput(videoExample.Id);
            var action = async () => await _useCase.Handle(input, CancellationToken.None);

            await action
                .Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Video '{videoExample.Id}' not found.");
             
            _videoRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(GetVideoWithAllProperties))]
        [Trait("Application", "GetVideo - Use Cases")]
        public async Task GetVideoWithAllProperties()
        {
            var videoExample = _fixture.GetValidVideoWithAllProperties();

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(x => x == videoExample.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(videoExample);

            var input = new UseCases.GetVideoInput(videoExample.Id);
            var output = await _useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(videoExample.Id);
            output.Title.Should().Be(videoExample.Title);
            output.Description.Should().Be(videoExample.Description);
            output.YearLaunched.Should().Be(videoExample.YearLaunched);
            output.Opened.Should().Be(videoExample.Opened);
            output.Published.Should().Be(videoExample.Published);
            output.Duration.Should().Be(videoExample.Duration);
            output.CreatedAt.Should().Be(videoExample.CreatedAt);
            output.Rating.Should().Be(videoExample.Rating.ToStringSignal());
            output.ThumbFileUrl.Should().Be(videoExample.Thumb!.Path); 
            output.ThumbHalfFileUrl.Should().Be(videoExample.ThumbHalf!.Path); 
            output.BannerFileUrl.Should().Be(videoExample.Banner!.Path);
            output.VideoFileUrl.Should().Be(videoExample.Media!.FilePath);
            output.TrailerFileUrl.Should().Be(videoExample.Trailer!.FilePath);
            output.CastMembersIds.Should().BeEquivalentTo(videoExample.CastMembers);
            output.CategoriesIds.Should().BeEquivalentTo(videoExample.Categories);
            output.GenresIds.Should().BeEquivalentTo(videoExample.Genres);
            _videoRepositoryMock.VerifyAll();
        }
    }
}
