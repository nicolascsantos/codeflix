using FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.DeleteGenre
{
    [CollectionDefinition(nameof(DeleteGenreAPITestFixture))]
    public class DeleteGenreAPITestFixtureCollection : ICollectionFixture<DeleteGenreAPITestFixture> { }

    public class DeleteGenreAPITestFixture : GenreBaseFixture
    {
    }
}
