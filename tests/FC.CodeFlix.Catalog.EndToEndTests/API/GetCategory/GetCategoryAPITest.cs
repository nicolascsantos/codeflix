using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FC.CodeFlix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.CodeFlix.Catalog.API.APIModels.Response;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.GetCategory
{
    [Collection(nameof(GetCategoryAPITestFixture))]
    public class GetCategoryAPITest
    {
        private readonly GetCategoryAPITestFixture _fixture;

        public GetCategoryAPITest(GetCategoryAPITestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(GetCategory))]
        [Trait("EndToEnd/API", "Category/GetCategoryById - Endpoints")]
        public async Task GetCategory()
        {
            var dbContext = _fixture.CreateDbContext();
            var categoriesExampleList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(categoriesExampleList);
            var exampleCategory = categoriesExampleList[10];


            var (response, output) = await _fixture.APIClient
                .Get<APIResponse<CategoryModelOutput>>($"/api/categories/{exampleCategory.Id}");
            response.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Data.Name.Should().Be(exampleCategory.Name);
            output.Data.Description.Should().Be(exampleCategory.Description);
            output.Data.IsActive.Should().Be(exampleCategory.IsActive);
            output.Data.CreatedAt.TrimMilliseconds().Should().Be(exampleCategory.CreatedAt.TrimMilliseconds());
        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryDoesntExist))]
        [Trait("EndToEnd/API", "Category/GetCategoryById - Endpoints")]
        public async Task ThrowWhenCategoryDoesntExist()
        {
            var categoriesExampleList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(categoriesExampleList);
            var randomGuid = Guid.NewGuid();
            var (response, output) = await _fixture.APIClient
                .Get<ProblemDetails>($"/api/categories/{randomGuid}");
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title = "One or more validation errors occurred.";
            output.Status = (int)StatusCodes.Status404NotFound;
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Category '{randomGuid}' not found.");
        }
    }
}
