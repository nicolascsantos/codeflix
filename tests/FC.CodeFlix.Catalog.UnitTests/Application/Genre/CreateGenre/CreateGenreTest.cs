using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreTestFixture))]
    public class CreateGenreTest
    {
        private readonly CreateGenreTestFixture _fixture;

        public CreateGenreTest(CreateGenreTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateGenre))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task CreateGenre()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var useCase = new UseCase.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
            var input = _fixture.GetExampleInput();

            var dateTimeBefore = DateTime.Now;
            var output = await useCase.Handle(input, CancellationToken.None);
            var dateTimeAfter = DateTime.Now.AddSeconds(1);

            genreRepositoryMock.Verify(x => x.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Once);
            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Categories.Should().HaveCount(0);
            output.Id.Should().NotBeEmpty();
            output.Id.Should().NotBe(default(Guid));
            (output.CreatedAt >= dateTimeBefore).Should().BeTrue();
            (output.CreatedAt <= dateTimeAfter).Should().BeTrue();
        }

        [Fact(DisplayName = nameof(CreateWithRelatedCategories))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task CreateWithRelatedCategories()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            var categoryRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var useCase = new UseCase.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
            var input = _fixture.GetExampleInputWithCategories();
            categoryRepositoryMock.Setup(i =>
                i.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()
            )).ReturnsAsync(input.CategoriesIds!);
            var output = await useCase.Handle(input, CancellationToken.None);

            genreRepositoryMock.Verify(x => x.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Once);
            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Id.Should().NotBe(default(Guid));
            output.Name.Should().Be(input.Name);
            output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? 0);
            input.CategoriesIds!.ForEach(id =>
                output.Categories.Should().Contain(relationId => relationId.Id == id)
            );
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateThrowWhenRelatedCategoryNotFound))]
        [Trait("Application", "CreateGenre - Use Cases")]
        public async Task CreateThrowWhenRelatedCategoryNotFound()
        {
            var input = _fixture.GetExampleInputWithCategories();
            var exampleGuid = input.CategoriesIds![^1];
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            categoryRepositoryMock.Setup(i =>
                i.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()
            )).ReturnsAsync(input.CategoriesIds.FindAll(x => x != exampleGuid));
            var useCase = new UseCase.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
            var action = async () => await useCase.Handle(input, CancellationToken.None);
            await action.Should()
                .ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id or ids not found: '{exampleGuid}'");
            categoryRepositoryMock.Verify(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
        [Trait("Application", "CreateGenre - Use Cases")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task ThrowWhenNameIsInvalid(string? invalidName)
        {
            var input = _fixture.GetExampleInput(invalidName);
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var categoryRepositoryMock = _fixture.GetCategoriesRepositoryMock();
            var unitOfWorkMock = _fixture.GetGenreUnitOfWorkMock();
            var useCase = new UseCase.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
            var action = async () => await useCase.Handle(input, CancellationToken.None);
            await action.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage($"Name should not be empty or null.");
        }
    }
}
