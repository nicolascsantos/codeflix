using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Moq;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.UpdateGenre
{
    [Collection(nameof(UpdateGenreTestFixture))]
    public class UpdateGenreTest
    {
        private readonly UpdateGenreTestFixture _fixture;

        public UpdateGenreTest(UpdateGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateGenre))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenre()
        {
            var exampleGenre = _fixture.GetValidGenre();
            var newName = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoriesRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);
            var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoriesRepositoryMock.Object);
            var input = new UseCase.UpdateGenreInput(newName, newIsActive);
            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(newName);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(0);
            genreRepositoryMock.Verify(repository => 
                repository.Update(It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id), It.IsAny<CancellationToken>()
            ), Times.Once);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
