using FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.UpdateGenre
{
    [CollectionDefinition(nameof(UpdateGenreAPITestFixture))]
    public class UpdateGenreTestFixtureCollection : ICollectionFixture<UpdateGenreAPITestFixture> { }

    public class UpdateGenreAPITestFixture : GenreBaseFixture
    {
    }
}
