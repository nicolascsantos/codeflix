using FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.CreateGenre
{
    [CollectionDefinition(nameof(CreateGenreAPITestFixture))]
    public class CreateGenreAPITestFixtureCollection : ICollectionFixture<CreateGenreAPITestFixture> { }

    public class CreateGenreAPITestFixture : GenreBaseFixture
    {
    }
}
