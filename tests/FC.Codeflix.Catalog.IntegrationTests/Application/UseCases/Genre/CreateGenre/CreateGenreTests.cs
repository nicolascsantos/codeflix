using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using FluentAssertions;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreTestFixture))]
    public class CreateGenreTests
    {
        private readonly CreateGenreTestFixture _fixture;

        public CreateGenreTests(CreateGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateGenre))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task CreateGenre()
        {
            CreateGenreInput input = _fixture.GetExampleInput();
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            GenreRepository genreRepository = new GenreRepository(dbContext);
            CategoryRepository categoryRepository = new CategoryRepository(dbContext);
            UnitOfWork unitOfWork = new UnitOfWork(dbContext);
            UseCase.CreateGenre createGenre = new UseCase.CreateGenre(genreRepository, unitOfWork, categoryRepository);

            var output = await createGenre.Handle(input, CancellationToken.None);

            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBe(default);
            output.Categories.Should().HaveCount(0);
        }

        [Fact(DisplayName = nameof(CreateGenreWithCategoriesRelations))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task CreateGenreWithCategoriesRelations()
        {
            List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(5);
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.SaveChangesAsync();
            CreateGenreInput input = _fixture.GetExampleInput();
            input.CategoriesIds = exampleCategories.Select(category => category.Id).ToList();
            CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
            GenreRepository genreRepository = new GenreRepository(actDbContext);
            CategoryRepository categoryRepository = new CategoryRepository(actDbContext);
            UnitOfWork unitOfWork = new UnitOfWork(actDbContext);
            UseCase.CreateGenre createGenre = new UseCase.CreateGenre(genreRepository, unitOfWork, categoryRepository);

            var output = await createGenre.Handle(input, CancellationToken.None);

            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.IsActive.Should().Be(input.IsActive);
            output.CreatedAt.Should().NotBe(default);
            output.Categories.Should().HaveCount(input.CategoriesIds.Count);
            output.Categories.Select(relation => relation.Id).Should().BeEquivalentTo(input.CategoriesIds);
            CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
            DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(output.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Name.Should().Be(input.Name);
            genreFromDb.IsActive.Should().Be(input.IsActive);
            List<GenresCategories> relations = await assertDbContext.GenresCategories
                .AsNoTracking()
                .Where(x => x.GenreId == output.Id).ToListAsync();
            relations.Should().HaveCount(input.CategoriesIds.Count);
            List<Guid> categoriesIdsRelatedFromDb = relations
                .Select(relation => relation.CategoryId)
                .ToList();
            categoriesIdsRelatedFromDb.Should().BeEquivalentTo(input.CategoriesIds);
        }

        [Fact(DisplayName = nameof(CreateGenreThrowsWhenCategoryDoesntExist))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task CreateGenreThrowsWhenCategoryDoesntExist()
        {
            List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(5);
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.SaveChangesAsync();
            CreateGenreInput input = _fixture.GetExampleInput();
            input.CategoriesIds = exampleCategories.Select(category => category.Id).ToList();
            Guid randomGuid = Guid.NewGuid();
            input.CategoriesIds.Add(randomGuid);
            CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
            GenreRepository genreRepository = new GenreRepository(actDbContext);
            CategoryRepository categoryRepository = new CategoryRepository(actDbContext);
            UnitOfWork unitOfWork = new UnitOfWork(actDbContext);
            UseCase.CreateGenre createGenre = new UseCase.CreateGenre(genreRepository, unitOfWork, categoryRepository);

            Func<Task<GenreModelOutput>> action = async () => await createGenre.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id or ids not found: '{randomGuid}'");
        }
    }
}
