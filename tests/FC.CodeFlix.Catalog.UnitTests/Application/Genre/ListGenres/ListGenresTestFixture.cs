using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.ListGenres
{
    [CollectionDefinition(nameof(ListGenresTestFixture))]
    public class ListGenresTestFixtureCollection : ICollectionFixture<ListGenresTestFixture> { }

    public class ListGenresTestFixture : GenreUseCasesBaseFixture
    {

    }
}
