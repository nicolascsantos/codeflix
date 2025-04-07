using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.UpdateGenre
{
    [CollectionDefinition(nameof(UpdateGenreTestFixture))]
    public class UpdateGenreTestFixtureCollection : ICollectionFixture<UpdateGenreTestFixture> { }

    public class UpdateGenreTestFixture : GenreUseCasesBaseFixture
    {

    }
}
