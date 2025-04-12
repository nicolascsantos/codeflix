using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Moq;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.GetGenre
{
    [Collection(nameof(GetGenreTestFixture))]
    public class GetGenreTest
    {
        private readonly GetGenreTestFixture _fixture;

        public GetGenreTest(GetGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(GetGenre))]
        [Trait("Application", "GetGenre - Use Cases")]
        public async Task GetGenre()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandomIdsList());
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);
            var useCase = new UseCase.GetGenre(genreRepositoryMock.Object);
            var input = new UseCase.GetGenreInput(exampleGenre.Id);
            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(exampleGenre.Name);
            output.IsActive.Should().Be(exampleGenre.IsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            foreach (var expectedId in output.Categories)
                output.Categories.Should().Contain(expectedId);

            genreRepositoryMock.Verify(repository =>
                repository.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "GetGenre - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandomIdsList());
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ThrowsAsync(new NotFoundException($"Genre '{exampleGenre.Id}' not found."));
            var useCase = new UseCase.GetGenre(genreRepositoryMock.Object);
            var input = new UseCase.GetGenreInput(exampleGenre.Id);
            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{exampleGenre.Id}' not found.");
        }
    }
}
