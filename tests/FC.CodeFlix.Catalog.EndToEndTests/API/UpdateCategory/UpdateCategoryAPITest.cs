using FC.CodeFlix.Catalog.API.APIModels.Category;
using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.UpdateCategory
{
    public class APIItemInput
    {
        public APIItemInput(string name, string? description = null, bool? isActive = null)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }

    [Collection(nameof(UpdateCategoryAPITestFixture))]
    public class UpdateCategoryAPITest : IClassFixture<UpdateCategoryAPITestFixture>
    {
        private readonly UpdateCategoryAPITestFixture _fixture;

        public UpdateCategoryAPITest(UpdateCategoryAPITestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateCategory))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategory()
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];
            var input = _fixture.GetExampleInput();

            var (response, output) = await _fixture.APIClient.Put<APIResponse<CategoryModelOutput>>(
                $"/api/categories/{exampleCategory.Id}",
                input
            );

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(input.Description);
            output.Data.IsActive.Should().Be((bool)input.IsActive!);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);
            var dbCategory = await _fixture.Persistence.GetById(exampleCategory.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be((bool)input.IsActive!);
        }

        [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategoryOnlyName()
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];
            var input = new UpdateCategoryAPIInput(
                _fixture.GetValidCategoryName()
            );

            var (response, output) = await _fixture.APIClient
                .Put<APIResponse<CategoryModelOutput>>($"/api/categories/{exampleCategory.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(exampleCategory.Description);
            output.Data.IsActive.Should().Be(exampleCategory.IsActive);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);
            var dbCategory = await _fixture.Persistence.GetById(exampleCategory.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(exampleCategory.Description);
            dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        }

        [Fact(DisplayName = nameof(UpdateCategoryOnlyNameAndDescription))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategoryOnlyNameAndDescription()
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];
            var input = new UpdateCategoryAPIInput(
                _fixture.GetValidCategoryName(),
                _fixture.GetValidCategoryDescription()
            );

            var (response, output) = await _fixture.APIClient
                .Put<APIResponse<CategoryModelOutput>>($"/api/categories/{exampleCategory.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(input.Description);
            output.Data.IsActive.Should().Be(exampleCategory.IsActive);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);
            var dbCategory = await _fixture.Persistence.GetById(exampleCategory.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        }

        [Fact(DisplayName = nameof(ErrorWhenNotFound))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task ErrorWhenNotFound()
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var randomGuid = Guid.NewGuid();
            var input = _fixture.GetExampleInput();

            var (response, output) = await _fixture.APIClient
                .Put<ProblemDetails>($"/api/categories/{randomGuid}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title.Should().Be("One or more validation errors occurred.");
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Category '{randomGuid}' not found.");
            output.Status.Should().Be(StatusCodes.Status404NotFound);
        }

        [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        [MemberData(nameof(UpdateCategoryAPITestDataGenerator.GetInvalidInputs), MemberType = typeof(UpdateCategoryAPITestDataGenerator))]
        public async Task ErrorWhenCantInstantiateAggregate(UpdateCategoryAPIInput input, string expectedDetail)
        {
            var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(exampleCategoriesList);
            var exampleCategory = exampleCategoriesList[10];

            var (response, output) = await _fixture.APIClient
                .Put<ProblemDetails>($"/api/categories/{exampleCategory.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output.Title.Should().Be("One or more validation errors occurred.");
            output.Type.Should().Be("UnprocessableEntity");
            output.Detail.Should().Be(expectedDetail);
            output.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        }
    }
}
