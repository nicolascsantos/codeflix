using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Video
{
    [Collection(nameof(VideoTestFixture))]
    public class VideoTest
    {
        private readonly VideoTestFixture _fixture;

        public VideoTest(VideoTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Video - Aggregates")]
        public void Instantiate()
        {
            var validVideo = _fixture.GetValidVideo();
            var expectedCreatedDate = DateTime.Now;
            var video = new DomainEntity.Video
            (
                validVideo.Title,
                validVideo.Description,
                validVideo.YearLaunched,
                validVideo.Opened, 
                validVideo.Published,
                validVideo.Duration
            );

            video.Title.Should().Be("Title");
            video.Description.Should().Be("Description");
            video.Opened.Should().Be(true);
            video.Published.Should().Be(true);
            video.YearLaunched.Should().Be(2001);
            video.Duration.Should().Be(180);
            video.CreatedAt.Should()
                .BeCloseTo(validVideo.CreatedAt, TimeSpan.FromSeconds(10));
        }

        [Fact(DisplayName = nameof(InstantiateThrowsWhenNotValid))]
        [Trait("Domain", "Video - Aggregates")]
        public void InstantiateThrowsWhenNotValid()
        {
            var validVideo = _fixture.GetValidVideo();
            var expectedCreatedDate = DateTime.Now;
            var action = () => new DomainEntity.Video
            (
                _fixture.GetTooLongTitle(),
                _fixture.GetTooLongDescription(),
                validVideo.YearLaunched,
                validVideo.Opened,
                validVideo.Published,
                validVideo.Duration
            );

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Validation errors.");
        }
    }
}
