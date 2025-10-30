using FC.CodeFlix.Catalog.Domain.Enum;
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
            video.Thumb.Should().BeNull();
            video.ThumbHalf.Should().BeNull();
            video.Banner.Should().BeNull();
            video.Media.Should().BeNull();
            video.Trailer.Should().BeNull();
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

        [Fact(DisplayName = nameof(UpdateThumb))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateThumb()
        {
            var validVideo = _fixture.GetValidVideo();
            var validImagePath = _fixture.GetValidImagePath();
            
            validVideo.UpdateThumb(validImagePath);

            validVideo.Should().NotBeNull();
            validVideo.Thumb!.Path.Should().Be(validImagePath);
        }

        [Fact(DisplayName = nameof(UpdateThumbHalf))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateThumbHalf()
        {
            var validVideo = _fixture.GetValidVideo();
            var validImagePath = _fixture.GetValidImagePath();

            validVideo.UpdateThumbHalf(validImagePath);

            validVideo.Should().NotBeNull();
            validVideo.ThumbHalf!.Path.Should().Be(validImagePath);
        }

        [Fact(DisplayName = nameof(UpdateBanner))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateBanner()
        {
            var validVideo = _fixture.GetValidVideo();
            var validImagePath = _fixture.GetValidImagePath();

            validVideo.UpdateBanner(validImagePath);

            validVideo.Banner.Should().NotBeNull();
            validVideo.Banner!.Path.Should().Be(validImagePath);
        }

        [Fact(DisplayName = nameof(UpdateMedia))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateMedia()
        {
            var validVideo = _fixture.GetValidVideo();
            var validMediaPath = _fixture.GetValidImagePath();

            validVideo.UpdateMedia(validMediaPath);

            validVideo.Media.Should().NotBeNull();
            validVideo.Media!.FilePath.Should().Be(validMediaPath);
        }

        [Fact(DisplayName = nameof(UpdateTrailer))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateTrailer()
        {
            var validVideo = _fixture.GetValidVideo();
            var validTrailerPath = _fixture.GetValidImagePath();

            validVideo.UpdateTrailer(validTrailerPath);

            validVideo.Trailer.Should().NotBeNull();
            validVideo.Trailer!.FilePath.Should().Be(validTrailerPath);
        }

        [Fact(DisplayName = nameof(UpdateAsSentToEncodeThrowsWhenThereIsNoMedia))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateAsSentToEncodeThrowsWhenThereIsNoMedia()
        {
            var validVideo = _fixture.GetValidVideo();
            var action = () => validVideo.UpdateAsSentToEncode();

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("There is no media.");
        }

        [Fact(DisplayName = nameof(UpdateAsEncoded))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateAsEncoded()
        {
            var validVideo = _fixture.GetValidVideo();

            var validMediaPath = _fixture.GetValidImagePath();

            validVideo.UpdateMedia(validMediaPath);

            validVideo.UpdateAsEncoded(validMediaPath);

            validVideo.Media.Should().NotBeNull();
            validVideo.Media.Status.Should().Be(MediaStatus.COMPLETED);
            validVideo.Media.EncodedPath.Should().Be(validMediaPath);
        }

        [Fact(DisplayName = nameof(UpdateAsEncodedThrowsWhenThereIsNoMedia))]
        [Trait("Domain", "Video - Aggregates")]
        public void UpdateAsEncodedThrowsWhenThereIsNoMedia()
        {
            var validVideo = _fixture.GetValidVideo();
            var validEncodedPath = _fixture.GetValidImagePath();
            var action = () => validVideo.UpdateAsEncoded(validEncodedPath);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("There is no media.");
        }

        [Fact(DisplayName = nameof(AddCategory))]
        [Trait("Domain", "Video - Aggregates")]
        public void AddCategory()
        {
            var validVideo = _fixture.GetValidVideo();
            var categoryIdExample = Guid.NewGuid();

            validVideo.AddCategory(categoryIdExample);

            validVideo.Categories.Should().HaveCount(1);
            validVideo.Categories[0].Should().Be(categoryIdExample);
        }

        [Fact(DisplayName = nameof(RemoveCategory))]
        [Trait("Domain", "Video - Aggregates")]
        public void RemoveCategory()
        {
            var validVideo = _fixture.GetValidVideo();
            var categoryIdExample = Guid.NewGuid();
            var categoryIdExample2 = Guid.NewGuid();
            validVideo.AddCategory(categoryIdExample);
            validVideo.AddCategory(categoryIdExample2);

            validVideo.RemoveCategory(categoryIdExample);


            validVideo.Categories.Should().HaveCount(1);
            validVideo.Categories[0].Should().Be(categoryIdExample2);
        }

        [Fact(DisplayName = nameof(RemoveAllCategories))]
        [Trait("Domain", "Video - Aggregates")]
        public void RemoveAllCategories()
        {
            var validVideo = _fixture.GetValidVideo();
            var categoryIdExample = Guid.NewGuid();
            var categoryIdExample2 = Guid.NewGuid();
            validVideo.AddCategory(categoryIdExample);
            validVideo.AddCategory(categoryIdExample2);

            validVideo.RemoveAllCategories();


            validVideo.Categories.Should().HaveCount(0);
        }
    }
}
