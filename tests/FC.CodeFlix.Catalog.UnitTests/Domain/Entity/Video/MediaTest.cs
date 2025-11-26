using FluentAssertions;
using FC.CodeFlix.Catalog.Domain.Enum;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Video
{
    [Collection(nameof(VideoTestFixture))]
    public class MediaTest
    {
        private readonly VideoTestFixture _fixture;
        public MediaTest(VideoTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Media - Entities")]
        public void Instantiate()
        {
            var expectedFilePath = _fixture.GetValidMediaPath();

            var media = new DomainEntity.Media(expectedFilePath);

            media.Should().NotBeNull();
            media.FilePath.Should().Be(expectedFilePath);
            media.Status.Should().Be(MediaStatus.PENDING);
        }

        [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
        [Trait("Domain", "Media - Entities")]
        public void UpdateAsSentToEncode()
        {
            var media = _fixture.GetValidMedia();

            media.UpdateAsSentToEncode();

            media.Status.Should().Be(MediaStatus.PROCESSING);
        }

        [Fact(DisplayName = nameof(UpdateAsEncoded))]
        [Trait("Domain", "Media - Entities")]
        public void UpdateAsEncoded()
        {
            var media = _fixture.GetValidMedia();

            var encodedExamplePath = _fixture.GetValidMediaPath();

            media.UpdateAsSentToEncode();
            media.UpdateAsEncoded(encodedExamplePath);

            media.Status.Should().Be(MediaStatus.COMPLETED);
            media.EncodedPath.Should().Be(encodedExamplePath);
        }
    }
}
