using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Validation;
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

            video.Title.Should().Be(validVideo.Title);
            video.Description.Should().Be(validVideo.Description);
            video.Opened.Should().Be(validVideo.Opened);
            video.Published.Should().Be(validVideo.Published);
            video.YearLaunched.Should().Be(validVideo.YearLaunched);
            video.Duration.Should().Be(validVideo.Duration);
            video.CreatedAt.Should()
                .BeCloseTo(validVideo.CreatedAt, TimeSpan.FromSeconds(10));
        }

        [Fact(DisplayName = nameof(ValidateWhenValidState))]
        [Trait("Domain", "Video - Aggregates")]
        public void ValidateWhenValidState()
        {
            var validVideo = _fixture.GetValidVideo();
            var notificationHandler = new NotificationValidationHandler();
            var validator = new VideoValidator(validVideo, notificationHandler);

            validVideo.Validate(notificationHandler);

            notificationHandler.HasErrors().Should().BeFalse();

        }

        [Fact(DisplayName = nameof(ValidateWhenInvalidState))]
        [Trait("Domain", "Video - Aggregates")]
        public void ValidateWhenInvalidState()
        {
            var validVideo = _fixture.GetValidVideo();
            var invalidVideo = new DomainEntity.Video
            (
                _fixture.GetTooLongTitle(),
                _fixture.GetTooLongDescription(),
                validVideo.YearLaunched,
                validVideo.Opened,
                validVideo.Published,
                validVideo.Duration
            );

            var notificationHandler = new NotificationValidationHandler();

            invalidVideo.Validate(notificationHandler);

            notificationHandler.HasErrors().Should().BeTrue();
            notificationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>()
            {
                new ValidationError("Title should be less or equal 255 characters long."),
                new ValidationError("Description should be less or equal 4000 characters long.")
            });
        }
    }
}
