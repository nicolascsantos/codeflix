using FC.CodeFlix.Catalog.UnitTests.Common;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Video
{
    [CollectionDefinition(nameof(VideoTestFixture))]
    public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture> { }

    public class VideoTestFixture : BaseFixture
    {
        public DomainEntity.Video GetValidVideo()
            => new("Title", "Description", 2001, true, true, 180);
        
    }
}
