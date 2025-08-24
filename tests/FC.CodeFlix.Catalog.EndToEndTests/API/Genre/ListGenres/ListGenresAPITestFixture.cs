using FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.ListGenres
{
    [CollectionDefinition(nameof(ListGenresAPITestFixture))]
    public class ListGenresAPITestFixtureCollection : ICollectionFixture<ListGenresAPITestFixture> { }

    public class ListGenresAPITestFixture : GenreBaseFixture
    {
    }
}
