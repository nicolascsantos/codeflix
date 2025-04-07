using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryTest))]
    public class DeleteCategoryTest : IClassFixture<DeleteCategoryTestFixture>
    {
        private readonly DeleteCategoryTestFixture _fixture;

        public DeleteCategoryTest(DeleteCategoryTestFixture fixture) => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Application", "DeleteCategory - UseCases")]
        public void DeleteCategory()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryExample = _fixture.GetExampleCategory();

            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(categoryExample);

            var input = new UseCases.Category.DeleteCategory.DeleteCategoryInput(categoryExample.Id);
            var useCase = new UseCases.Category.DeleteCategory.DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var output = useCase.Handle(input, CancellationToken.None);
            repositoryMock.Verify(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.Delete(categoryExample, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
        [Trait("Application", "DeleteCategory - UseCases")]
        public void ThrowWhenCategoryNotFound()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var categoryExample = _fixture.GetExampleCategory();
            var guidExample = Guid.NewGuid();
            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException($"Category '{guidExample}' not found"));
            var input = new UseCases.Category.DeleteCategory.DeleteCategoryInput(guidExample);
            var useCase = new UseCases.Category.DeleteCategory.DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);
            var task = async () => await useCase.Handle(input, CancellationToken.None);
            task.Should().ThrowAsync<NotFoundException>();
            repositoryMock.Verify(x => x.Get(guidExample, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
