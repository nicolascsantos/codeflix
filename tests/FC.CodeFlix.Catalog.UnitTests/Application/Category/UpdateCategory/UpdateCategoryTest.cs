using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTest))]
    public class UpdateCategoryTest : IClassFixture<UpdateCategoryTestFixture>
    {
        private readonly UpdateCategoryTestFixture _fixture;

        public UpdateCategoryTest(UpdateCategoryTestFixture fixture) => _fixture = fixture;


        [Theory(DisplayName = nameof(UpdateCategory))]
        [Trait("Application", "UpdateCategory - UseCases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 10, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategory(DomainEntity.Category exampleCategory, UpdateCategoryInput input)
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be((bool)input.IsActive!);
            repositoryMock.Verify(repository => repository.Update(exampleCategory, It.IsAny<CancellationToken>()));
            unitOfWorkMock.Verify(i => i.Commit(It.IsAny<CancellationToken>()));
        }

        [Theory(DisplayName = nameof(UpdateCategoryWithoutProvidingIsActive))]
        [Trait("Application", "UpdateCategory - UseCases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 10, MemberType = typeof(UpdateCategoryTestDataGenerator))]

        public async Task UpdateCategoryWithoutProvidingIsActive(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            CategoryModelOutput output = await useCase.Handle(exampleInput, CancellationToken.None);
            output.Should().NotBeNull();
            output.Name.Should().Be(exampleInput.Name);
            output.Description.Should().Be(exampleInput.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);
            repositoryMock.Verify(repository => repository.Update(exampleCategory, It.IsAny<CancellationToken>()));
            unitOfWorkMock.Verify(i => i.Commit(It.IsAny<CancellationToken>()));
        }

        [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
        [Trait("Application", "UpdateCategory - UseCases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate), parameters: 10, MemberType = typeof(UpdateCategoryTestDataGenerator))]

        public async Task UpdateCategoryOnlyName(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
        {
            var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            CategoryModelOutput output = await useCase.Handle(exampleInput, CancellationToken.None);
            output.Should().NotBeNull();
            output.Name.Should().Be(exampleInput.Name);
            output.Description.Should().Be(exampleCategory.Description);
            output.IsActive.Should().Be(exampleCategory.IsActive);
            repositoryMock.Verify(repository => repository.Update(exampleCategory, It.IsAny<CancellationToken>()));
            unitOfWorkMock.Verify(i => i.Commit(It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = nameof(UpdateThrowWhenCategoryNotFound))]
        [Trait("Application", "UpdateCategory - UseCases")]
        public async Task UpdateThrowWhenCategoryNotFound()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var input = _fixture.GetValidInput();
            repositoryMock.Setup(i => i.Get(input.Id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException($"Category '{input.Id}' not found"));
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var task = async () => await useCase.Handle(input, CancellationToken.None);
            await task.Should().ThrowAsync<NotFoundException>();
            repositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(UpdateThrowWhenCantUpdateCategory))]
        [Trait("Application", "UpdateCategory - UseCases")]
        [MemberData(nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs), parameters: 12, MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public void UpdateThrowWhenCantUpdateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
        {
            var exampleCategory = _fixture.GetExampleCategory();
            input.Id = exampleCategory.Id;
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            repositoryMock.Setup(i => i.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var task = async () => await useCase.Handle(input, CancellationToken.None);
            task.Should().ThrowAsync<EntityValidationException>()
                .WithMessage(expectedExceptionMessage);
            repositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
