using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Xunit.Abstractions;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.ListCategories
{
    [Collection(nameof(ListCategoriesAPITestFixture))]
    public class ListCategoriesAPITest : IClassFixture<ListCategoriesAPITestFixture>, IDisposable
    {
        private readonly ListCategoriesAPITestFixture _fixture;
        private readonly ITestOutputHelper _output;

        public ListCategoriesAPITest(ListCategoriesAPITestFixture fixture, ITestOutputHelper output)
            => (_fixture, _output) = (fixture, output);



        [Fact(DisplayName = nameof(ListCategoriesAndTotalWithDefault))]
        [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
        public async Task ListCategoriesAndTotalWithDefault()
        {
            var defaultPerPage = 15;
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(exampleCategoriesList);


            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CategoryModelOutput>>("/api/categories");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Meta.Total.Should().Be(exampleCategoriesList.Count);
            output.Meta.CurrentPage.Should().Be(1);
            output.Meta.PerPage.Should().Be(defaultPerPage);
            output.Data.Should().HaveCount(defaultPerPage);
            foreach (CategoryModelOutput outputItem in output.Data)
            {
                var exampleItem = exampleCategoriesList.Find(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            }
        }

        [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
        [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
        public async Task ItemsEmptyWhenPersistenceEmpty()
        {
            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CategoryModelOutput>>("/api/categories");
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Total.Should().Be(0);
            output.Data.Should().HaveCount(0);
        }

        [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
        [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
        public async Task ListCategoriesAndTotal()
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(page: 1, perPage: 5);

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CategoryModelOutput>>("/api/categories", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output.Meta.Total.Should().Be(exampleCategoriesList.Count);
            output.Data.Should().HaveCount(input.PerPage);
            foreach (CategoryModelOutput outputItem in output.Data)
            {
                var exampleItem = exampleCategoriesList.Find(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            }
        }

        [Theory(DisplayName = nameof(ListPaginated))]
        [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListPaginated(
            int quantityCategoriesToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems)
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var input = new ListCategoriesInput(page: page, perPage: perPage);

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CategoryModelOutput>>("/api/categories", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output.Meta.Total.Should().Be(exampleCategoriesList.Count);
            output.Data.Should().HaveCount(expectedQuantityItems);
            foreach (CategoryModelOutput outputItem in output.Data)
            {
                var exampleItem = exampleCategoriesList.Find(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            }
        }

        [Theory(DisplayName = nameof(ListOrdered))]
        [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdat", "asc")]
        [InlineData("createdat", "desc")]
        [InlineData("", "asc")]
        public async Task ListOrdered(string orderBy, string order)
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var input = new ListCategoriesInput
            (
                page: 1,
                perPage: 15,
                search: "",
                sort: orderBy,
                dir: inputOrder
            );

            var count = 0;
            var expectedArray = exampleCategoriesList.Select(x => $" {++count} {x.Name};{x.CreatedAt};{JsonConvert.SerializeObject(x)}");

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CategoryModelOutput>>("/api/categories", input);
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Meta.Total.Should().Be(exampleCategoriesList.Count);
            output.Data.Should().HaveCount(exampleCategoriesList.Count);
            var expectedOrderedList = _fixture.CloneCategoriesListOrdered(exampleCategoriesList, input.Sort, input.Dir);
            count = 0;
            var outputArray = output.Data.Select(x => $" {++count} {x.Name};{x.CreatedAt};{JsonConvert.SerializeObject(x)}");
            _output.WriteLine("Expected...");
            _output.WriteLine(string.Join(";\n", expectedArray));
            _output.WriteLine("Outputs...");
            _output.WriteLine(string.Join(";\n", outputArray));
            foreach (CategoryModelOutput outputItem in output.Data)
            {
                var exampleItem = exampleCategoriesList.FirstOrDefault(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
            }
        }


        [Theory(DisplayName = nameof(ListOrderedDates))]
        [Trait("EndToEnd/API", "Category/ListCategories - Endpoints")]
        [InlineData("createdat", "asc")]
        [InlineData("createdat", "desc")]
        public async Task ListOrderedDates(string orderBy, string order)
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var input = new ListCategoriesInput
            (
                page: 1,
                perPage: 15,
                search: "",
                sort: orderBy,
                dir: inputOrder
            );

            var count = 0;
            var expectedArray = exampleCategoriesList.Select(x => $" {++count} {x.Name};{x.CreatedAt.TrimMilliseconds()};{JsonConvert.SerializeObject(x)}");

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CategoryModelOutput>>("/api/categories", input);
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Meta.Total.Should().Be(exampleCategoriesList.Count);
            output.Data.Should().HaveCount(exampleCategoriesList.Count);
            DateTime? lastItemDate = null;
            count = 0;
            var outputArray = output.Data.Select(x => $" {++count} {x.Name};{x.CreatedAt.TrimMilliseconds()};{JsonConvert.SerializeObject(x)}");
            _output.WriteLine("Expected...");
            _output.WriteLine(string.Join(";\n", expectedArray));
            _output.WriteLine("Outputs...");
            _output.WriteLine(string.Join(";\n", outputArray));
            foreach (CategoryModelOutput outputItem in output.Data)
            {
                var exampleItem = exampleCategoriesList.Find(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());
                if (lastItemDate != null)
                {
                    if (order == "asc")
                        Assert.True(outputItem.CreatedAt.TrimMilliseconds() >= lastItemDate);
                    else
                        Assert.True(outputItem.CreatedAt.TrimMilliseconds() <= lastItemDate);
                }
                lastItemDate = outputItem.CreatedAt.TrimMilliseconds();
            }
        }


        public void Dispose()
        {
            _fixture.CleanPersistence();
        }
    }
}
