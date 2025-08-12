using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre
{
    [Collection(nameof(UpdateGenreTestFixture))]
    public class UpdateGenreTests
    {
        private readonly UpdateGenreTestFixture _fixture;

        public UpdateGenreTests(UpdateGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateGenre))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task UpdateGenre()
        {
            List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            DomainEntity.Genre targetGenre = exampleGenres[5];
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.SaveChangesAsync();

            CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
            GenreRepository genreRepository = new GenreRepository(actDbContext);
            CategoryRepository categoryRepository = new CategoryRepository(actDbContext);
            UnitOfWork unitOfWork = new UnitOfWork(actDbContext);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(genreRepository, unitOfWork, categoryRepository);
            var input = new UseCase.UpdateGenreInput(targetGenre.Id, _fixture.GetValidGenreName(), !targetGenre.IsActive);

            var output = await updateGenre.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be((bool)input.IsActive!);
            CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
            DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Id.Should().Be(targetGenre.Id);
            genreFromDb.Name.Should().Be(input.Name);
            genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        }

        [Fact(DisplayName = nameof(UpdateGenreWithCategoriesRelations))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task UpdateGenreWithCategoriesRelations()
        {
            List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList();
            List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            DomainEntity.Genre targetGenre = exampleGenres[5];
            List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
            List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            List<GenresCategories> relations = targetGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
                .ToList();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(relations);
            await arrangeDbContext.SaveChangesAsync();

            CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
            GenreRepository genreRepository = new GenreRepository(actDbContext);
            CategoryRepository categoryRepository = new CategoryRepository(actDbContext);
            UnitOfWork unitOfWork = new UnitOfWork(actDbContext);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(genreRepository, unitOfWork, categoryRepository);
            var input = new UseCase.UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive,
                newRelatedCategories.Select(category => category.Id).ToList()
            );

            var output = await updateGenre.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be((bool)input.IsActive!);
            output.Categories.Should().HaveCount(newRelatedCategories.Count);
            List<Guid> relatedCategoryIdsFromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
            relatedCategoryIdsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
            CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
            DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Id.Should().Be(targetGenre.Id);
            genreFromDb.Name.Should().Be(input.Name);
            genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
            List<Guid> relatedCategoriesIdsFromDb = await assertDbContext.GenresCategories
                .AsNoTracking()
                .Where(relation => relation.GenreId == input.Id)
                .Select(relation => relation.CategoryId)
                .ToListAsync();
            relatedCategoriesIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);

        }

        [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategoryDoesntExist))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task UpdateGenreThrowsWhenCategoryDoesntExist()
        {
            List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList();
            List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            DomainEntity.Genre targetGenre = exampleGenres[5];
            List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
            List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            List<GenresCategories> relations = targetGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
                .ToList();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(relations);
            await arrangeDbContext.SaveChangesAsync();

            CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
            GenreRepository genreRepository = new GenreRepository(actDbContext);
            CategoryRepository categoryRepository = new CategoryRepository(actDbContext);
            UnitOfWork unitOfWork = new UnitOfWork(actDbContext);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(genreRepository, unitOfWork, categoryRepository);
            List<Guid> categoriesIdsToRelate = newRelatedCategories.Select(category => category.Id).ToList();
            Guid invalidCategoryId = Guid.NewGuid();
            categoriesIdsToRelate.Add(invalidCategoryId);
            var input = new UseCase.UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive,
                categoriesIdsToRelate
            );

            Func<Task<GenreModelOutput>> action =
                async () => await updateGenre.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id or ids not found: '{invalidCategoryId}'");
        }

        [Fact(DisplayName = nameof(UpdateGenreThrowsWhenGenreNotFound))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task UpdateGenreThrowsWhenGenreNotFound()
        {
            List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.SaveChangesAsync();

            CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
            GenreRepository genreRepository = new GenreRepository(actDbContext);
            CategoryRepository categoryRepository = new CategoryRepository(actDbContext);
            UnitOfWork unitOfWork = new UnitOfWork(actDbContext);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(genreRepository, unitOfWork, categoryRepository);
            Guid randomGuid = Guid.NewGuid();
            var input = new UseCase.UpdateGenreInput(randomGuid, _fixture.GetValidGenreName(), true);

            Func<Task<GenreModelOutput>> output = async () 
                => await updateGenre.Handle(input, CancellationToken.None);

            await output.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(UpdateGenreWithoutNewCategoriesRelations))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task UpdateGenreWithoutNewCategoriesRelations()
        {
            List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList();
            List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            DomainEntity.Genre targetGenre = exampleGenres[5];
            List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            List<GenresCategories> relations = targetGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
                .ToList();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(relations);
            await arrangeDbContext.SaveChangesAsync();

            CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
            GenreRepository genreRepository = new GenreRepository(actDbContext);
            CategoryRepository categoryRepository = new CategoryRepository(actDbContext);
            UnitOfWork unitOfWork = new UnitOfWork(actDbContext);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(genreRepository, unitOfWork, categoryRepository);
            var input = new UseCase.UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive
            );

            var output = await updateGenre.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be((bool)input.IsActive!);
            output.Categories.Should().HaveCount(relatedCategories.Count);
            List<Guid> expectedRelatedCategoriesIds = relatedCategories.Select(category => category.Id).ToList();
            List<Guid> relatedCategoryIdsFromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
            relatedCategoryIdsFromOutput.Should().BeEquivalentTo(expectedRelatedCategoriesIds);
            CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
            DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Id.Should().Be(targetGenre.Id);
            genreFromDb.Name.Should().Be(input.Name);
            genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
            List<Guid> relatedCategoriesIdsFromDb = await assertDbContext.GenresCategories
                .AsNoTracking()
                .Where(relation => relation.GenreId == input.Id)
                .Select(relation => relation.CategoryId)
                .ToListAsync();
            relatedCategoriesIdsFromDb.Should().BeEquivalentTo(expectedRelatedCategoriesIds);

        }

        [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesIdsCleanRelations))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task UpdateGenreWithEmptyCategoriesIdsCleanRelations()
        {
            List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList();
            List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            DomainEntity.Genre targetGenre = exampleGenres[5];
            List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
            relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            List<GenresCategories> relations = targetGenre.Categories
                .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
                .ToList();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(relations);
            await arrangeDbContext.SaveChangesAsync();

            CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
            GenreRepository genreRepository = new GenreRepository(actDbContext);
            CategoryRepository categoryRepository = new CategoryRepository(actDbContext);
            UnitOfWork unitOfWork = new UnitOfWork(actDbContext);
            UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(genreRepository, unitOfWork, categoryRepository);
            var input = new UseCase.UpdateGenreInput(
                targetGenre.Id,
                _fixture.GetValidGenreName(),
                !targetGenre.IsActive,
                new List<Guid>()
            );

            var output = await updateGenre.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(targetGenre.Id);
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be((bool)input.IsActive!);
            output.Categories.Should().HaveCount(0);
            List<Guid> relatedCategoryIdsFromOutput = output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
            relatedCategoryIdsFromOutput.Should().BeEquivalentTo(new List<Guid>());
            CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
            DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Id.Should().Be(targetGenre.Id);
            genreFromDb.Name.Should().Be(input.Name);
            genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
            List<Guid> relatedCategoriesIdsFromDb = await assertDbContext.GenresCategories
                .AsNoTracking()
                .Where(relation => relation.GenreId == input.Id)
                .Select(relation => relation.CategoryId)
                .ToListAsync();
            relatedCategoriesIdsFromDb.Should().BeEquivalentTo(new List<Guid>());

        }
    }
}
