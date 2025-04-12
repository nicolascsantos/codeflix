using FluentAssertions;
using Moq;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.ListGenres
{
    public class ListGenresTest
    {
        private readonly ListGenresTestFixture _fixture;

        public ListGenresTest(ListGenresTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ListGenres))]
        [Trait("Application", "ListGenres - Use Cases")]
        public async Task ListGenres()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var exampleGenresList = _fixture.GetExampleGenresList();

        }
    }
}
