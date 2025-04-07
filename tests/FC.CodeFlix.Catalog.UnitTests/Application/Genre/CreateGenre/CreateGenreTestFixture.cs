using FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common;
using Moq;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.CreateGenre
{
    [CollectionDefinition(nameof(CreateGenreTestFixture))]
    public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture>
    {

    }

    public class CreateGenreTestFixture : GenreUseCasesBaseFixture
    {

    }
}
