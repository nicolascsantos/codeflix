using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.DeleteGenre
{
    [Collection(nameof(DeleteGenreTestFixture))]
    public class DeleteGenreTest
    {
        private readonly DeleteGenreTestFixture _fixture;

        public DeleteGenreTest(DeleteGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteGenre))]
        [Trait("Application", "DeleteGenre - Use Cases")]
        public async Task DeleteGenre()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            var exampleGenre = _fixture.GetExampleGenre();

            genreRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenre);

            var useCase = new UseCase.DeleteGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
            var input = new UseCase.DeleteGenreInput(exampleGenre.Id);
            var output = await useCase.Handle(input, CancellationToken.None);

            genreRepositoryMock.Verify(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
            genreRepositoryMock.Verify(x => x.Delete(exampleGenre, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(DeleteGenreWhenGenreNotFound))]
        [Trait("Application", "DeleteGenre - Use Cases")]
        public async Task DeleteGenreWhenGenreNotFound()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            var genreExample = _fixture.GetValidGenre();
            genreRepositoryMock.Setup(x =>
                x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()
            )).ThrowsAsync(new NotFoundException($"Genre '{genreExample.Id}' not found."));
            var input = new UseCase.DeleteGenreInput(genreExample.Id);
            var useCase = new UseCase.DeleteGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action
                .Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{genreExample.Id}' not found.");

            genreRepositoryMock.Verify(x => x.Get(genreExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
