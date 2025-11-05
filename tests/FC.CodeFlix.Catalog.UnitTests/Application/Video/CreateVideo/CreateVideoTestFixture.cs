using FC.CodeFlix.Catalog.UnitTests.Common.Fixtures;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.CreateVideo
{
    [CollectionDefinition(nameof(CreateVideoTestFixture))]
    public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture> { }

    public class CreateVideoTestFixture : VideoTestFixtureBase
    {
    }
}
