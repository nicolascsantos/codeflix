
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre
{
    [CollectionDefinition(nameof(DeleteGenreTestFixure))]
    public class DeleteGenreTestFixureCollection : ICollectionFixture<DeleteGenreTestFixure> { }

    public class DeleteGenreTestFixure : GenreUseCaseBaseFixture
    {
    }
}
