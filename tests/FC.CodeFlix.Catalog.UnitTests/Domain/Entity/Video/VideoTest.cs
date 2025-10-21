using FC.CodeFlix.Catalog.Domain.Enum;
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
            var expectedTitle = _fixture.GetValidTitle();
            var expectedDescription = _fixture.GetValidDescription();
            var expectedYearLaunched = _fixture.GetValidYearLaunched();
            var expectedOpened = _fixture.GetRandomBoolean();
            var expectedPublished = _fixture.GetRandomBoolean();
            var expectedDuration = _fixture.GetValidDuration();
            var expectedCreatedAt = DateTime.Now;
            var expectedRating = Rating.ER;

            var video = new DomainEntity.Video
            (
                expectedTitle,
                expectedDescription,
                expectedYearLaunched,
                expectedOpened,
                expectedPublished,
                expectedDuration,
                expectedRating
            );

            video.Title.Should().Be(expectedTitle);
            video.Description.Should().Be(expectedDescription);
            video.Opened.Should().Be(expectedOpened);
            video.Published.Should().Be(expectedPublished);
            video.YearLaunched.Should().Be(expectedYearLaunched);
            video.Duration.Should().Be(expectedDuration);
            video.CreatedAt.Should()
                .BeCloseTo(expectedCreatedAt, TimeSpan.FromSeconds(10));
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
                validVideo.Duration,
                validVideo.Rating
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

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Video - Aggregates")]
        public void Update()
        {
            var expectedTitle = _fixture.GetValidTitle();
            var expectedDescription = _fixture.GetValidDescription();
            var expectedYearLaunched = _fixture.GetValidYearLaunched();
            var expectedOpened = _fixture.GetRandomBoolean();
            var expectedPublished = _fixture.GetRandomBoolean();
            var expectedDuration = _fixture.GetValidDuration();
            var video = _fixture.GetValidVideo();

            video.Update(
                expectedTitle,
                expectedDescription,
                expectedYearLaunched,
                expectedOpened,
                expectedPublished,
                expectedDuration
            );

            video.Title.Should().Be(expectedTitle);
            video.Description.Should().Be(expectedDescription);
            video.YearLaunched.Should().Be(expectedYearLaunched);
            video.Opened.Should().Be(expectedOpened);
            video.Published.Should().Be(expectedPublished);
            video.Duration.Should().Be(expectedDuration);
        }

        [Fact(DisplayName = nameof(UpdateValidation))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateValidation()
        {
            var expectedTitle = _fixture.GetValidTitle();
            var expectedDescription = _fixture.GetValidDescription();
            var expectedYearLaunched = _fixture.GetValidYearLaunched();
            var expectedOpened = _fixture.GetRandomBoolean();
            var expectedPublished = _fixture.GetRandomBoolean();
            var expectedDuration = _fixture.GetValidDuration();
            var video = _fixture.GetValidVideo();

            video.Update(
                expectedTitle,
                expectedDescription,
                expectedYearLaunched,
                expectedOpened,
                expectedPublished,
                expectedDuration
            );

            var notificationHandler = new NotificationValidationHandler();

            video.Validate(notificationHandler);

            notificationHandler.HasErrors().Should().BeFalse();
        }

        [Fact(DisplayName = nameof(UpdateValidationWhenInvalidState))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateValidationWhenInvalidState()
        {
            var expectedTitle = _fixture.GetTooLongTitle();
            var expectedDescription = _fixture.GetTooLongDescription();
            var expectedYearLaunched = _fixture.GetValidYearLaunched();
            var expectedOpened = _fixture.GetRandomBoolean();
            var expectedPublished = _fixture.GetRandomBoolean();
            var expectedDuration = _fixture.GetValidDuration();
            var video = _fixture.GetValidVideo();

            video.Update(
                expectedTitle,
                expectedDescription,
                expectedYearLaunched,
                expectedOpened,
                expectedPublished,
                expectedDuration
            );

            var notificationHandler = new NotificationValidationHandler();

            video.Validate(notificationHandler);

            notificationHandler.HasErrors().Should().BeTrue();
            notificationHandler.Errors.Should().HaveCount(2);
            notificationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>()
            {
                new ValidationError("Title should be less or equal 255 characters long."),
                new ValidationError("Description should be less or equal 4000 characters long.")
            });
        }
    }
}
