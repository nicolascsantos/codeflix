using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.GetGenre
{
    [CollectionDefinition(nameof(GetGenreAPITestFixture))]
    public class GetGenreAPITestFixtureCollection : ICollectionFixture<GetGenreAPITestFixture> { }

    public class GetGenreAPITestFixture : GenreBaseFixture
    {
    }
}
