using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre
{
    [Collection(nameof(GetGenreTestFixture))]
    public class GetGenreTest
    {
        private readonly GetGenreTestFixture _fixture;

        public GetGenreTest(GetGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(GetGenre))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task GetGenre()
        {
            var exampleGenre = _fixture.GetExampleGenre();
            var dbContext = _fixture.CreateDbContext();
            await dbContext.Genres.AddAsync(exampleGenre);
            await dbContext.SaveChangesAsync();

            var genreRepository = new GenreRepository(dbContext);
            var useCase = new UseCase.GetGenre(genreRepository);

            var output = await useCase.Handle(new UseCase.GetGenreInput(exampleGenre.Id), CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(exampleGenre.Id);
            output.Name.Should().Be(exampleGenre.Name);
            output.IsActive.Should().Be(exampleGenre.IsActive);
            output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            output.Categories.Should().HaveCount(exampleGenre.Categories.Count);
        }

        [Fact(DisplayName = nameof(GetGenreThrowWhenNotFound))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task GetGenreThrowWhenNotFound()
        {
            var exampleGenre = _fixture.GetExampleGenre();
            var dbContext = _fixture.CreateDbContext();


            var genreRepository = new GenreRepository(dbContext);
            var useCase = new UseCase.GetGenre(genreRepository);

            var action = async () => await useCase.Handle(new UseCase.GetGenreInput(exampleGenre.Id), CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Genre '{exampleGenre.Id}' not found.");
        }

        [Fact(DisplayName = nameof(GetGenreWithRelations))]
        [Trait("Integration/Application", "GetGenre - Use Cases")]
        public async Task GetGenreWithRelations()
        {
            var genresExampleList = _fixture.GetExampleListGenres(10);
            var dbContext = _fixture.CreateDbContext();
            var categoriesExamplesList = _fixture.GetExampleCategoriesList(5);
            var expectedGenre = genresExampleList[5];
            categoriesExamplesList.ForEach(category => expectedGenre.AddCategory(category.Id));
            await dbContext.Categories.AddRangeAsync(categoriesExamplesList);
            await dbContext.Genres.AddRangeAsync(genresExampleList);
            await dbContext.GenresCategories
                .AddRangeAsync(expectedGenre.Categories.Select(categoryId => new GenresCategories(categoryId, expectedGenre.Id)));
            await dbContext.SaveChangesAsync();

            var genreRepository = new GenreRepository(dbContext);
            var useCase = new UseCase.GetGenre(genreRepository);

            var output = await useCase.Handle(new UseCase.GetGenreInput(expectedGenre.Id), CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(expectedGenre.Id);
            output.Name.Should().Be(expectedGenre.Name);
            output.IsActive.Should().Be(expectedGenre.IsActive);
            output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
            output.Categories.Should().HaveCount(expectedGenre.Categories.Count);
            output.Categories.ToList().ForEach(relationModel =>
            {
                expectedGenre.Categories.Should().Contain(relationModel.Id);
                relationModel.Name.Should().BeNull();
            });
        }
    }
}
