using FC.CodeFlix.Catalog.UnitTests.Common.Fixtures;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Video
{
    [CollectionDefinition(nameof(VideoTestFixture))]
    public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture> { }

    public class VideoTestFixture : VideoTestFixtureBase
    {
    }
}
