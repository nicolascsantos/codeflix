using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre
{
    [Collection(nameof(DeleteGenreTestFixure))]
    public class DeleteGenreTests
    {
        private readonly DeleteGenreTestFixure _fixture;

        public DeleteGenreTests(DeleteGenreTestFixure fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteGenre))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task DeleteGenre()
        {
            var genresExampleList = _fixture.GetExampleListGenres();
            var targetGenre = genresExampleList[5];
            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.SaveChangesAsync();

            var actDbContext = _fixture.CreateDbContext();
            var useCase = new UseCase.DeleteGenre(
                new GenreRepository(actDbContext),
                new UnitOfWork(actDbContext)
            );
            var input = new DeleteGenreInput(targetGenre.Id);

            var output = useCase.Handle(input, CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContext();
            var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().BeNull();
        }

        [Fact(DisplayName = nameof(DeleteGenreThrowsWhenGenreNotFound))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task DeleteGenreThrowsWhenGenreNotFound()
        {
            var genresExampleList = _fixture.GetExampleListGenres();
            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.SaveChangesAsync();

            var actDbContext = _fixture.CreateDbContext();
            var useCase = new UseCase.DeleteGenre(
                new GenreRepository(actDbContext),
                new UnitOfWork(actDbContext)
            );

            var randomGuid = Guid.NewGuid();
            var input = new DeleteGenreInput(randomGuid);

            Func<Task<Unit>> output = async () =>
                await useCase.Handle(input, CancellationToken.None);

            await output.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task DeleteGenreWithRelations()
        {
            var genresExampleList = _fixture.GetExampleListGenres();
            var targetGenre = genresExampleList[5];
            var exampleCategories = _fixture.GetExampleCategoriesList(5);
            exampleCategories.ForEach(category => targetGenre.AddCategory(category.Id));
            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(genresExampleList);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(exampleCategories.Select(category => new GenresCategories(category.Id, targetGenre.Id)));
            await arrangeDbContext.SaveChangesAsync();

            var actDbContext = _fixture.CreateDbContext();
            var useCase = new UseCase.DeleteGenre(
                new GenreRepository(actDbContext),
                new UnitOfWork(actDbContext)
            );
            var input = new DeleteGenreInput(targetGenre.Id);

            var output = useCase.Handle(input, CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContext();
            var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
            genreFromDb.Should().BeNull();
            var relations = assertDbContext.GenresCategories
                .AsNoTracking()
                .Where(id => id.GenreId == targetGenre.Id)
                .ToList();
            relations.Should().HaveCount(0);
        }
    }
}
