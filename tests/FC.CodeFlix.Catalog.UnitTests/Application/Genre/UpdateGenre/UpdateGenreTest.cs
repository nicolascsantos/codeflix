using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
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
            var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newName, newIsActive);
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

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var exampleId = Guid.NewGuid();
            genreRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found."));

            var useCase = new UseCase.UpdateGenre(
                genreRepositoryMock.Object,
                _fixture.GetGenreUnitOfWorkMock().Object,
                _fixture.GetCategoriesRepositoryMock().Object
            );

            var input = new UseCase.UpdateGenreInput(exampleId, _fixture.GetValidGenreName());

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleId}' not found.");
        }

        [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ThrowWhenNameIsInvalid(string? name)
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
            var input = new UseCase.UpdateGenreInput(exampleGenre.Id, name!, newIsActive);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage($"Name should not be empty or null.");
        }

        [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateGenreOnlyName(bool isActive)
        {
            var exampleGenre = _fixture.GetExampleGenre(isActive: isActive);
            var newName = _fixture.GetValidGenreName();
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoriesRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);
            var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoriesRepositoryMock.Object);
            var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newName);
            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(newName);
            output.IsActive.Should().Be(isActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(0);
            genreRepositoryMock.Verify(repository =>
                repository.Update(It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id), It.IsAny<CancellationToken>()
            ), Times.Once);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(UpdateGenreAddingCategoriesIds))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreAddingCategoriesIds()
        {
            var exampleGenre = _fixture.GetValidGenre();
            var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
            var newName = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoriesRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);
            categoriesRepositoryMock.Setup(x =>
                x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleCategoriesIdsList);
            var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoriesRepositoryMock.Object);
            var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newName, newIsActive, exampleCategoriesIdsList);
            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(newName);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
            exampleCategoriesIdsList.ForEach
            (
                expectedId => output.Categories.Should().Contain(expectedId)
            );
            genreRepositoryMock.Verify(repository =>
                repository.Update(It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id), It.IsAny<CancellationToken>()
            ), Times.Once);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesIds))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreReplacingCategoriesIds()
        {
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandomIdsList());
            var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
            var newName = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoriesRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);
            categoriesRepositoryMock.Setup(x =>
                x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleCategoriesIdsList);
            var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoriesRepositoryMock.Object);
            var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newName, newIsActive, exampleCategoriesIdsList);
            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(newName);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
            exampleCategoriesIdsList.ForEach
            (
                expectedId => output.Categories.Should().Contain(expectedId)
            );
            genreRepositoryMock.Verify(repository =>
                repository.Update(It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id), It.IsAny<CancellationToken>()
            ), Times.Once);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task ThrowWhenCategoryNotFound()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoriesRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandomIdsList());

            var exampleNewCategoriesIdsList = _fixture.GetRandomIdsList(10);
            var listReturnedByCategoryRepository = exampleNewCategoriesIdsList.GetRange(0, exampleNewCategoriesIdsList.Count - 2);
            var idsNotReturnedByCategoryRepository = exampleNewCategoriesIdsList.GetRange(exampleNewCategoriesIdsList.Count - 2, 2);
            var newName = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);
            categoriesRepositoryMock.Setup(x =>
                x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()
            )).ReturnsAsync(listReturnedByCategoryRepository);
            var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoriesRepositoryMock.Object);
            var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newName, newIsActive, exampleNewCategoriesIdsList);
            var action = async () => await useCase.Handle(input, CancellationToken.None);
            var notFoundIdsAsString = string.Join(';', idsNotReturnedByCategoryRepository);
            await action.Should()
                .ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id or ids not found: '{notFoundIdsAsString}'");
        }

        [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithoutCategoriesIds()
        {
            var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: exampleCategoriesIdsList);
            var newName = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoriesRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);
            var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoriesRepositoryMock.Object);
            var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newName, newIsActive);
            GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(newName);
            output.IsActive.Should().Be(newIsActive);
            output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
            exampleCategoriesIdsList.ForEach
            (
                expectedId => output.Categories.Should().Contain(expectedId)
            );
            genreRepositoryMock.Verify(repository =>
                repository.Update(It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id), It.IsAny<CancellationToken>()
            ), Times.Once);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesIdsList))]
        [Trait("Application", "UpdateGenre - Use Cases")]
        public async Task UpdateGenreWithEmptyCategoriesIdsList()
        {
            var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
            var exampleGenre = _fixture.GetExampleGenre(categoriesIds: exampleCategoriesIdsList);
            var newName = _fixture.GetValidGenreName();
            var newIsActive = !exampleGenre.IsActive;
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoriesRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            genreRepositoryMock.Setup(i =>
                i.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);
            var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoriesRepositoryMock.Object);
            var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newName, newIsActive, new List<Guid>());
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
