using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenre
{
    [Collection(nameof(ListGenreTestFixture))]
    public class ListGenreTests
    {
        private readonly ListGenreTestFixture _fixture;

        public ListGenreTests(ListGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ListGenres))]
        [Trait("Application", "ListGenre - Use Cases")]
        public async Task ListGenres()
        {
            var exampleGenresList = _fixture.GetExampleListGenres(10);
            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenresList);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.ListGenres(
                new GenreRepository(actDbContext),
                new CategoryRepository(actDbContext)
            );
            var input = new ListGenresInput(
                page: 1,
                perPage: 20
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleGenresList.Count);
            output.Items.Should().HaveCount(exampleGenresList.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                DomainEntity.Genre exampleItem = exampleGenresList.Find(example => example.Id == outputItem.Id)!;
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            });
        }

        [Fact(DisplayName = nameof(ListGenreReturnsEmptyWhenPersistenceIsEmpty))]
        [Trait("Application", "ListGenre - Use Cases")]
        public async Task ListGenreReturnsEmptyWhenPersistenceIsEmpty()
        {

            var arrangeDbContext = _fixture.CreateDbContext();

            var useCase = new UseCase.ListGenres(new GenreRepository(arrangeDbContext), new CategoryRepository(arrangeDbContext));
            var input = new ListGenresInput(
                page: 1,
                perPage: 20
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().HaveCount(0);
        }

        [Fact(DisplayName = nameof(ListGenreVerifyRelations))]
        [Trait("Application", "ListGenre - Use Cases")]
        public async Task ListGenreVerifyRelations()
        {
            var exampleGenres = _fixture.GetExampleListGenres(10);
            var exampleCategories = _fixture.GetExampleCategoriesList(10);
            var random = new Random();
            exampleGenres.ForEach(genre =>
            {
                int relationsCount = random.Next(0, 3);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selectedCategory = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selectedCategory.Id))
                        genre.AddCategory(selectedCategory.Id);
                }
            });

            List<GenresCategories> genresCategories = new List<GenresCategories>();

            exampleGenres.ForEach(
                genre => genre.Categories.ToList().ForEach(
                    categoryId => genresCategories.Add(
                        new GenresCategories(categoryId, genre.Id)
                    )
                )
            );

            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(genresCategories);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
            var input = new ListGenresInput(
                page: 1,
                perPage: 20
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleGenres.Count);
            output.Items.Should().HaveCount(exampleGenres.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                DomainEntity.Genre exampleItem = exampleGenres.Find(example => example.Id == outputItem.Id)!;
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
                List<Guid> outputItemCategoriesIds = outputItem.Categories.Select(id => id.Id).ToList();
                outputItemCategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
                outputItem.Categories.ToList().ForEach(outputCategory =>
                {
                    DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputCategory.Name.Should().Be(exampleCategory.Name);
                });
            });
        }

        [Theory(DisplayName = nameof(ListGenrePaginated))]
        [Trait("Application", "ListGenre - Use Cases")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListGenrePaginated(int quantityToGenerate, int page, int perPage, int expectedQuantityItems)
        {
            var exampleGenres = _fixture.GetExampleListGenres(quantityToGenerate);
            var exampleCategories = _fixture.GetExampleCategoriesList(10);
            var random = new Random();
            exampleGenres.ForEach(genre =>
            {
                int relationsCount = random.Next(0, 3);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selectedCategory = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selectedCategory.Id))
                        genre.AddCategory(selectedCategory.Id);
                }
            });

            List<GenresCategories> genresCategories = new List<GenresCategories>();

            exampleGenres.ForEach(
                genre => genre.Categories.ToList().ForEach(
                    categoryId => genresCategories.Add(
                        new GenresCategories(categoryId, genre.Id)
                    )
                )
            );

            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(genresCategories);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
            var input = new ListGenresInput(
                page: page,
                perPage: perPage
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleGenres.Count);
            output.Items.Should().HaveCount(expectedQuantityItems);
            output.Items.ToList().ForEach(outputItem =>
            {
                DomainEntity.Genre exampleItem = exampleGenres.Find(example => example.Id == outputItem.Id)!;
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
                List<Guid> outputItemCategoriesIds = outputItem.Categories.Select(id => id.Id).ToList();
                outputItemCategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
                outputItem.Categories.ToList().ForEach(outputCategory =>
                {
                    DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputCategory.Name.Should().Be(exampleCategory.Name);
                });
            });
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("Application", "ListGenre - Use Cases")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(string search, int page, int perPage, int expectedQuantityItemsReturned, int expectedQuantityTotalItems)
        {
            var exampleGenreList = _fixture.GetExampleListGenresByNames(new List<string>()
            {
                "Action",
                "Horror",
                "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Sci-fi AI",
                "Sci-fi Space",
                "Sci-fi Robots",
                "Sci-fi Future"
            });
            var exampleCategories = _fixture.GetExampleCategoriesList(10);
            var random = new Random();
            exampleGenreList.ForEach(genre =>
            {
                int relationsCount = random.Next(0, 3);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selectedCategory = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selectedCategory.Id))
                        genre.AddCategory(selectedCategory.Id);
                }
            });

            List<GenresCategories> genresCategories = new List<GenresCategories>();

            exampleGenreList.ForEach(
                genre => genre.Categories.ToList().ForEach(
                    categoryId => genresCategories.Add(
                        new GenresCategories(categoryId, genre.Id)
                    )
                )
            );

            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(genresCategories);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
            var input = new ListGenresInput(
                page: page,
                perPage: perPage,
                search: search
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            output.Items.ToList().ForEach(outputItem =>
            {
                DomainEntity.Genre exampleItem = exampleGenreList.Find(example => example.Id == outputItem.Id)!;
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Contain(search);
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
                List<Guid> outputItemCategoriesIds = outputItem.Categories.Select(id => id.Id).ToList();
                outputItemCategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
                outputItem.Categories.ToList().ForEach(outputCategory =>
                {
                    DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputCategory.Name.Should().Be(exampleCategory.Name);
                });
            });
        }

        [Theory(DisplayName = nameof(SearchOrdered))]
        [Trait("Application", "ListGenre - Use Cases")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdat", "asc")]
        [InlineData("createdat", "desc")]
        [InlineData("", "asc")]
        public async Task SearchOrdered(string orderBy, string order)
        {
            var exampleGenres = _fixture.GetExampleListGenres(10);
            var exampleCategories = _fixture.GetExampleCategoriesList(10);
            var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var random = new Random();
            exampleGenres.ForEach(genre =>
            {
                int relationsCount = random.Next(0, 3);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selectedCategory = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selectedCategory.Id))
                        genre.AddCategory(selectedCategory.Id);
                }
            });

            List<GenresCategories> genresCategories = new List<GenresCategories>();

            exampleGenres.ForEach(
                genre => genre.Categories.ToList().ForEach(
                    categoryId => genresCategories.Add(
                        new GenresCategories(categoryId, genre.Id)
                    )
                )
            );

            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.Genres.AddRangeAsync(exampleGenres);
            await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
            await arrangeDbContext.GenresCategories.AddRangeAsync(genresCategories);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var useCase = new UseCase.ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));
            var input = new ListGenresInput(
                page: 1,
                perPage: 20,
                sort: orderBy,
                dir: searchOrder
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneGenreListOrdered(
               exampleGenres,
               orderBy,
               searchOrder
           );

            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleGenres.Count);
            output.Items.Should().HaveCount(exampleGenres.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                DomainEntity.Genre exampleItem = exampleGenres.Find(example => example.Id == outputItem.Id)!;
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
                List<Guid> outputItemCategoriesIds = outputItem.Categories.Select(id => id.Id).ToList();
                outputItemCategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
                outputItem.Categories.ToList().ForEach(outputCategory =>
                {
                    DomainEntity.Category? exampleCategory = exampleCategories.Find(x => x.Id == outputCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputCategory.Name.Should().Be(exampleCategory.Name);
                });
            });

            for (int indice = 0; indice < expectedOrderedList.Count; indice++)
            {
                var expectedItem = expectedOrderedList[indice];
                var outputItem = output.Items[indice];
                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }
    }
}
