using FC.CodeFlix.Catalog.Application.UseCases.Video.GetVideo;
using FC.CodeFlix.Catalog.UnitTests.Common.Fixtures;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.GetVideo
{
    [CollectionDefinition(nameof(GetVideoTestFixture))]
    public class GetVideoTestFixtureCollection : ICollectionFixture<GetVideoTestFixture> { }

    public class GetVideoTestFixture : VideoTestFixtureBase
    {
        public GetVideoInput GetValidGetVideoInput(Guid? videoId = null)
            => new(videoId ?? Guid.NewGuid());
    }
}
