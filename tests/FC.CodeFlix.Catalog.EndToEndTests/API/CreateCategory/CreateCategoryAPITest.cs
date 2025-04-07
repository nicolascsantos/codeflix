using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.API.Category.CreateCategory
{
    [Collection(nameof(CreateCategoryAPITestFixture))]
    public class CreateCategoryAPITest
    {
        private readonly CreateCategoryAPITestFixture _fixture;

        public CreateCategoryAPITest(CreateCategoryAPITestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("EndToEnd/API", "Category/Create - Endpoints")]
        public async Task CreateCategory()
        {
            var input = _fixture.GetExampleInput();
            var (response, output) = await _fixture.APIClient.Post<APIResponse<CategoryModelOutput>>("/api/categories", input);
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.Created);
            output.Should().NotBeNull();
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(input.Description);
            output.Data.IsActive.Should().Be(input.IsActive);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);
            var dbCategory = await _fixture.Persistence.GetById(output.Data.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(output.Data.Name);
            dbCategory.Description.Should().Be(output.Data.Description);
            dbCategory.IsActive.Should().Be(output.Data.IsActive);
            dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
        [Trait("EndToEnd/API", "Category/Create - Endpoints")]
        [MemberData(
            nameof(CreateCategoryAPITestDataGenerator.GetInvalidInputs),
            parameters: 12,
            MemberType = typeof(CreateCategoryAPITestDataGenerator)
        )]
        public async Task ErrorWhenCantInstantiateAggregate(CreateCategoryInput input, string expectedDetails)
        {
            var (response, output) = await _fixture.APIClient.Post<ProblemDetails>("/api/categories", input);
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            output.Should().NotBeNull();
            output.Title.Should().Be("One or more validation errors occurred.");
            output.Type.Should().Be("UnprocessableEntity");
            output.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
            output.Detail.Should().Be(expectedDetails);
        }
    }
}
