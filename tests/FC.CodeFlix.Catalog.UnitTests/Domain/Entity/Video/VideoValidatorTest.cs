using FC.CodeFlix.Catalog.Domain.Validation;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Video
{
    [Collection(nameof(VideoTestFixture))]
    public class VideoValidatorTest
    {
        private readonly VideoTestFixture _fixture;

        public VideoValidatorTest(VideoTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ReturnsValidWhenVideoIsValid))]
        [Trait("Domain", "Video Validator - Validators")]
        public void ReturnsValidWhenVideoIsValid()
        {
            var validVideo = _fixture.GetValidVideo();
            var notificationValidationHandler = new NotificationValidationHandler();
            var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

            videoValidator.Validate();

            notificationValidationHandler.HasErrors().Should().BeFalse();
            notificationValidationHandler.Errors.Should().HaveCount(0);
        }
    }
}
