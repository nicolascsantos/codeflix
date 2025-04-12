using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.GetGenre
{
    [CollectionDefinition(nameof(GetGenreTestFixture))]
    public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture>
    {}

    public class GetGenreTestFixture : GenreUseCasesBaseFixture
    {
    }
}
