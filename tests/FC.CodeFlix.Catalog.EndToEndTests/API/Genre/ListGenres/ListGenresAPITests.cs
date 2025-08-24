using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.ListGenres
{
    [Collection(nameof(ListGenresAPITestFixture))]
    public class ListGenresAPITests : IDisposable
    {
        private readonly ListGenresAPITestFixture _fixture;

        public ListGenresAPITests(ListGenresAPITestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ListGenres))]
        [Trait("EndToEnd/API", "Genre/List - Endpoints")]
        public async Task ListGenres()
        {
            List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres();
            await _fixture.Persistence.InsertList(exampleGenres);

            var input = new ListGenresInput(
                page: 1,
                perPage: exampleGenres.Count
            );

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<GenreModelOutput>>("/api/genres", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.Total.Should().Be(exampleGenres.Count);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Count.Should().Be(input.PerPage);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            });
        }

        [Fact(DisplayName = nameof(EmptyList))]
        [Trait("EndToEnd/API", "Genre/List - Endpoints")]
        public async Task EmptyList()
        {
            var input = new ListGenresInput(
                page: 1,
                perPage: 15
            );

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<GenreModelOutput>>("/api/genres", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.Total.Should().Be(0);
            output.Data.Count.Should().Be(0);
        }

        [Theory(DisplayName = nameof(ListGenresPaginated))]
        [Trait("EndToEnd/API", "Genre/List - Endpoints")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListGenresPaginated(
            int quantityToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
            )
        {
            List<DomainEntity.Genre> exampleGenres = _fixture
                .GetExampleListGenres(quantityToGenerate);
            await _fixture.Persistence.InsertList(exampleGenres);

            var input = new ListGenresInput(
                page: page,
                perPage: perPage
            );

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<GenreModelOutput>>("/api/genres", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.Total.Should().Be(quantityToGenerate);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Count.Should().Be(expectedQuantityItems);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            });
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("EndToEnd/API", "Genre/List - Endpoints")]
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
            var exampleGenres = _fixture.GetExampleListGenresByNames(
                new List<string>()
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
                }
            );

            await _fixture.Persistence.InsertList(exampleGenres);

            var input = new ListGenresInput(
                page: page,
                perPage: perPage,
                search: search
            );

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<GenreModelOutput>>("/api/genres", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.Total.Should().Be(expectedQuantityTotalItems);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Count.Should().Be(expectedQuantityItemsReturned);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            });
        }

        [Theory(DisplayName = nameof(SearchOrdered))]
        [Trait("EndToEnd/API", "Genre/List - Endpoints")]
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
            await _fixture.Persistence.InsertList(exampleGenres);

            var orderEnum = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;

            var input = new ListGenresInput(
                page: 1,
                perPage: 10,
                sort: orderBy,
                dir: orderEnum
            );

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<GenreModelOutput>>("/api/genres", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.Total.Should().Be(10);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Count.Should().Be(10);
            var expectedOrderedList = _fixture
                .CloneGenreListOrdered(exampleGenres, order, orderEnum);

            for (int indice = 0; indice < expectedOrderedList.Count; indice++)
            {
                var outputItem = output.Data[indice];
                var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            }
        }


        [Fact(DisplayName = nameof(ListGenresWithRelations))]
        [Trait("EndToEnd/API", "Genre/List - Endpoints")]
        public async Task ListGenresWithRelations()
        {
            List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres();
            List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList();
            List<GenresCategories> genresCategories = new List<GenresCategories>();

            await _fixture
                .GenresListWithCategoriesRelations(
                exampleGenres,
                exampleCategories,
                genresCategories,
                _fixture);

            var input = new ListGenresInput(
                page: 1,
                perPage: exampleGenres.Count
            );

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<GenreModelOutput>>("/api/genres", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.Total.Should().Be(exampleGenres.Count);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Count.Should().Be(input.PerPage);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
                var relatedCategories = outputItem.Categories.Select(x => x.Id).ToList();
                relatedCategories.Should().BeEquivalentTo(exampleItem.Categories);
                outputItem.Categories.ToList().ForEach(outputRelatedCategory =>
                {
                    var exampleCategory = exampleCategories
                        .Find(x => x.Id == outputRelatedCategory.Id);
                    exampleCategory.Should().NotBeNull();
                    outputRelatedCategory.Name.Should().Be(exampleCategory.Name);
                });
            });
        }


        public void Dispose()
            => _fixture.CleanPersistence();
    }
}
