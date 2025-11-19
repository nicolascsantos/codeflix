using FC.CodeFlix.Catalog.UnitTests.Common.Fixtures;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.ListVideos
{
    [CollectionDefinition(nameof(ListVideosTestFixture))]
    public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture> { }

    public class ListVideosTestFixture : VideoTestFixtureBase
    {
        public List<DomainEntity.Video> GetVideosExamplesList(int length = 10)
            => Enumerable.Range(1, Random.Shared.Next(2, length))
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();
        
    }
}
