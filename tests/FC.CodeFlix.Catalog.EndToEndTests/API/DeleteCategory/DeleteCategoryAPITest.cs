using FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryAPITestFixture))]
    public class DeleteCategoryAPITest : IClassFixture<DeleteCategoryAPITestFixture>
    {
        private readonly DeleteCategoryAPITestFixture _fixture;

        public DeleteCategoryAPITest(DeleteCategoryAPITestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
        public async Task DeleteCategory()
        {
            var categoriesList = _fixture.GetExampleCategoriesList(20);
            await _fixture.Persistence.InsertList(categoriesList);
            var categoryToDelete = categoriesList[10];
            var input = new DeleteCategoryInput(categoryToDelete.Id);
            var (response, output) = await _fixture.APIClient
                .Delete<object>($"/api/categories/{categoryToDelete.Id}");
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();
            var persistenceCategory = await _fixture.Persistence.GetById(categoryToDelete.Id);
            persistenceCategory.Should().BeNull();
        }

        [Fact(DisplayName = nameof(ErrorWhenCategoryNotFound))]
        [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
        public async Task ErrorWhenCategoryNotFound()
        {
            var categoriesExampleList = _fixture.GetExampleCategoriesList();
            await _fixture.Persistence.InsertList(categoriesExampleList);
            var randomGuid = Guid.NewGuid();
            var input = new DeleteCategoryInput(randomGuid);
            var (response, output) = await _fixture.APIClient
                .Delete<ProblemDetails>($"/api/categories/{randomGuid}");
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title.Should().Be("One or more validation errors occurred.");
            output.Status.Should().Be((int)StatusCodes.Status404NotFound);
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Category '{randomGuid}' not found.");
        }
    }
}
