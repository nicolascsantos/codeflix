using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryTest))]
    public class DeleteCategoryTest : IClassFixture<DeleteCategoryTestFixture>
    {
        private readonly DeleteCategoryTestFixture _fixture;

        public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Integration/Application", "DeleteCategory - UseCases")]
        public async Task DeleteCategory()
        {
            var dbContext = _fixture.CreateDbContext();
            var categoryExample = _fixture.GetExampleCategory();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var exampleList = _fixture.GetExampleCategoriesList();
            await dbContext.Categories.AddRangeAsync(exampleList);
            var trackingInfo = await dbContext.Categories.AddAsync(categoryExample);
            await dbContext.SaveChangesAsync();
            trackingInfo.State = EntityState.Detached;
            var input = new UseCase.DeleteCategoryInput(categoryExample.Id);
            var useCase = new UseCase.DeleteCategory(repository, unitOfWork);
            var output = useCase.Handle(input, CancellationToken.None);
            var assertedDbContext = _fixture.CreateDbContext(true);
            var deletedCategory = await dbContext.Categories.FindAsync(categoryExample.Id);
            deletedCategory.Should().BeNull();
            var dbCategories = await assertedDbContext.Categories.ToListAsync();
            dbCategories.Should().HaveCount(exampleList.Count);
        }


        [Fact(DisplayName = nameof(DeleteCategoryThrowWhenCategoryNotFound))]
        [Trait("Integration/Application", "DeleteCategory - UseCases")]
        public async Task DeleteCategoryThrowWhenCategoryNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var exampleList = _fixture.GetExampleCategoriesList();
            await dbContext.Categories.AddRangeAsync(exampleList);
            var guidExample = Guid.NewGuid();
            
            var input = new UseCase.DeleteCategoryInput(guidExample);
            var useCase = new UseCase.DeleteCategory(repository, unitOfWork);
            var task = async () => await useCase.Handle(input, CancellationToken.None);
            await task.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Category '{guidExample.ToString()}' not found.");
        }
    }
}
