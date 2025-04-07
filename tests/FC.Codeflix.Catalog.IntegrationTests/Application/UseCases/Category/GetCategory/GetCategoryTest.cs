using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory.Common;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory
{
    [Collection(nameof(GetCategoryTest))]
    public class GetCategoryTest : IClassFixture<CategoryUseCasesBaseFixture>
    {
        private readonly CategoryUseCasesBaseFixture _fixture;

        public GetCategoryTest(CategoryUseCasesBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(GetCategory))]
        [Trait("Integration/Application", "GetCategory - Use Cases")]
        public async Task GetCategory()
        {
            var exampleCategory = _fixture.GetExampleCategory();
            var dbContext = _fixture.CreateDbContext();
            dbContext.Categories.Add(exampleCategory);
            dbContext.SaveChanges();
            var repository = new CategoryRepository(dbContext);
            var input = new UseCase.GetCategoryInput(exampleCategory.Id);
            var useCase = new UseCase.GetCategory(repository);
            var output = await useCase.Handle(input, CancellationToken.None);
            output.Should().NotBeNull();
            output.Name.Should().Be(exampleCategory.Name);
            output.Description.Should().Be(exampleCategory.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);
            output.Id.Should().Be(exampleCategory.Id);
            output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        }

        [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesntExist))]
        [Trait("Integration/Application", "GetCategory - Use Cases")]
        public async Task NotFoundExceptionWhenCategoryDoesntExist()
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var exampleCategory = _fixture.GetExampleCategory();
            dbContext.Categories.Add(exampleCategory);
            dbContext.SaveChanges();

            var useCase = new UseCase.GetCategory(repository);
            var input = new UseCase.GetCategoryInput(Guid.NewGuid());

            var task = async ()
                => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{input.Id}' not found.");
        }
    }
}
