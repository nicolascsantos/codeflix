using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories
{
    [Collection(nameof(ListCategoriesTest))]
    public class ListCategoriesTest : IClassFixture<ListCategoriesTestFixture>
    {
        private readonly ListCategoriesTestFixture _fixture;

        public ListCategoriesTest(ListCategoriesTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
        [Trait("Integration/Application", "ListCategories - UseCases")]
        public async Task SearchReturnsListAndTotal()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
            exampleCategoriesList.Add(exampleCategory);
            await dbContext.AddRangeAsync(exampleCategoriesList);
            await dbContext.SaveChangesAsync();
            var categoryRepository = new CategoryRepository(dbContext);

            var input = new UseCase.ListCategoriesInput(
                page: 1,
                perPage: 20
            );

            var useCase = new UseCase.ListCategories(categoryRepository);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleCategoriesList.Count);
            output.Items.Should().HaveCount(exampleCategoriesList.Count);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.Find(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
        [Trait("Integration/Application", "ListCategories - UseCases")]
        public async Task SearchReturnsEmptyWhenEmpty()
        {
            var dbContext = _fixture.CreateDbContext();
            var categoryRepository = new CategoryRepository(dbContext);
            var useCase = new UseCase.ListCategories(categoryRepository);
            var input = new ListCategoriesInput(
                page: 1,
                perPage: 20
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.Total.Should().Be(0);
            output.Items.Should().HaveCount(0);
        }

        [Theory(DisplayName = nameof(SearchReturnsPaginated))]
        [Trait("Integration/Application", "ListCategories - UseCases")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPaginated(
            int quantityCategoriesToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
            )
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
            await dbContext.AddRangeAsync(exampleCategoriesList);
            await dbContext.SaveChangesAsync();
            var categoryRepository = new CategoryRepository(dbContext);
            var useCase = new UseCase.ListCategories(categoryRepository);

            var input = new ListCategoriesInput(
                page: page,
                perPage: perPage,
                search: ""
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(quantityCategoriesToGenerate);
            output.Items.Should().HaveCount(expectedQuantityItems);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = exampleCategoriesList.Find(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("Integration/Application", "ListCategories - UseCases")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
        )
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            var categoriesNamesList = _fixture.GetExampleCategoriesListWithNames(new List<string>()
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
            categoriesNamesList.Add(exampleCategory);
            await dbContext.AddRangeAsync(categoriesNamesList);
            await dbContext.SaveChangesAsync();
            var categoryRepository = new CategoryRepository(dbContext);
            var useCase = new UseCase.ListCategories(categoryRepository);

            var input = new ListCategoriesInput(
                page: page,
                perPage: perPage,
                search: search
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var exampleItem = categoriesNamesList.Find(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchOrdered))]
        [Trait("Integration/Application", "ListCategories - UseCases")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdat", "asc")]
        [InlineData("createdat", "desc")]
        [InlineData("", "asc")]
        public async Task SearchOrdered(string orderBy, string order)
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategoriesList = _fixture.GetExampleCategoriesList();
            var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            await dbContext.AddRangeAsync(exampleCategoriesList);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            var categoryRepository = new CategoryRepository(dbContext);
            var useCase = new UseCase.ListCategories(categoryRepository);

            var input = new ListCategoriesInput(
                page: 1,
                perPage: 20,
                search: "",
                sort: orderBy, 
                dir: searchOrder
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
                exampleCategoriesList,
                orderBy,
                searchOrder
            );


            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(exampleCategoriesList.Count);
            output.Items.Should().HaveCount(exampleCategoriesList.Count);
            for (int indice = 0; indice < expectedOrderedList.Count; indice++)
            {
                var expectedItem = expectedOrderedList[indice];
                var outputItem = output.Items[indice];
                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }
    }
}
