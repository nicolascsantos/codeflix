using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.DeleteGenre
{
    [CollectionDefinition(nameof(DeleteGenreTestFixture))]
    public class DeleteGenreTestCollection : ICollectionFixture<DeleteGenreTestFixture> { }

    public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
    {
    }
}
