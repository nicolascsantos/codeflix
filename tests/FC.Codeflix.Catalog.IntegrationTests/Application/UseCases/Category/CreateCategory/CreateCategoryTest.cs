using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory
{
    public class CreateCategoryTest : IClassFixture<CreateCategoryTestFixture>
    {
        private readonly CreateCategoryTestFixture _fixture;

        public CreateCategoryTest(CreateCategoryTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async Task CreateCategory()
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.CreateCategory(repository, unitOfWork);
            var input = _fixture.GetInput();
            var output = await useCase.Handle(input, CancellationToken.None);
            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(input.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async Task CreateCategoryWithOnlyName()
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.CreateCategory(repository, unitOfWork);
            var input = new CreateCategoryInput(_fixture.GetValidCategoryName());
            var output = await useCase.Handle(input, CancellationToken.None);
            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be("");
            dbCategory.IsActive.Should().BeTrue();
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be("");
            output.IsActive.Should().BeTrue();
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async Task CreateCategoryWithOnlyNameAndDescription()
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.CreateCategory(repository, unitOfWork);
            var input = new CreateCategoryInput(_fixture.GetValidCategoryName(), _fixture.GetValidCategoryDescription());
            var output = await useCase.Handle(input, CancellationToken.None);
            var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().BeTrue();
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().BeTrue();
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        [MemberData(
            nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
            parameters: 4,
            MemberType = typeof(CreateCategoryTestDataGenerator)
        )]
        public async Task ThrowWhenCantInstantiateCategory(CreateCategoryInput input, string expectedExceptionMessage)
        {
            var dbContext = _fixture.CreateDbContext();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.CreateCategory(repository, unitOfWork);
            var task = async () => await useCase.Handle(input, CancellationToken.None);
            await task.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage(expectedExceptionMessage);

            var dbCategoriesList = _fixture.CreateDbContext(true).Categories
                .AsNoTracking()
                .ToList();
            dbCategoriesList.Should().HaveCount(0);
        }
    }
}
