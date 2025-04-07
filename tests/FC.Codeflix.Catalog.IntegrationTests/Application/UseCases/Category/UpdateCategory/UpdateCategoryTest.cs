using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTest))]
    public class UpdateCategoryTest : IClassFixture<UpdateCategoryTestFixture>
    {
        private readonly UpdateCategoryTestFixture _fixture;

        public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
            => _fixture = fixture;

        [Theory(DisplayName = nameof(UpdateCategory))]
        [Trait("Integration/Application", "UpdateCategory - UseCases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
            parameters: 5,
            MemberType = typeof(UpdateCategoryTestDataGenerator)
        )]
        public async Task UpdateCategory(DomainEntity.Category exampleCategory, UpdateCategoryInput input)
        {
            var dbContext = _fixture.CreateDbContext();
            await dbContext.Categories.AddRangeAsync(_fixture.GetExampleCategoriesList());
            var trackingInfo = await dbContext.Categories.AddAsync(exampleCategory);
            await dbContext.SaveChangesAsync();
            trackingInfo.State = EntityState.Detached;
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);
            var output = await useCase.Handle(input, CancellationToken.None);
            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be((bool)input.IsActive!);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be((bool)input.IsActive!);
        }

        [Theory(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
        [Trait("Integration/Application", "UpdateCategory - UseCases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
            parameters: 5,
            MemberType = typeof(UpdateCategoryTestDataGenerator)
        )]
        public async Task UpdateCategoryWithoutIsActive(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
            var dbContext = _fixture.CreateDbContext();
            await dbContext.Categories.AddRangeAsync(_fixture.GetExampleCategoriesList());
            var trackingInfo = await dbContext.Categories.AddAsync(exampleCategory);
            await dbContext.SaveChangesAsync();
            trackingInfo.State = EntityState.Detached;
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);
            var output = await useCase.Handle(input, CancellationToken.None);
            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);
        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
        [Trait("Integration/Application", "UpdateCategory - UseCases")]
        public async Task ThrowWhenCategoryNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var input = _fixture.GetValidInput();
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);
            var task = async () => await useCase.Handle(input, CancellationToken.None);
            await task.Should().ThrowAsync<NotFoundException>();
        }

        [Theory(DisplayName = nameof(ThrowWhenCantUpdateCategory))]
        [Trait("Integration/Application", "UpdateCategory - UseCases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs), parameters: 12, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public void ThrowWhenCantUpdateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategory = _fixture.GetExampleCategory();
            input.Id = exampleCategory.Id;
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);
            var task = async () => await useCase.Handle(input, CancellationToken.None);
            task.Should().ThrowAsync<EntityValidationException>()
                .WithMessage(expectedExceptionMessage);
        }
    }
}
