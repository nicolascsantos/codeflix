using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre
{
    [CollectionDefinition(nameof(CreateGenreTestFixture))]
    public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }
    public class CreateGenreTestFixture : GenreUseCaseBaseFixture
    {
    }
}
