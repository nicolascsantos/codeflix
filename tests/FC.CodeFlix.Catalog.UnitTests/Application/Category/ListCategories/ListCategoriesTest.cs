using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.ListCategories
{
    [Collection(nameof(ListCateogoriesTestFixture))]
    public class ListCategoriesTest : IClassFixture<ListCateogoriesTestFixture>
    {
        private readonly ListCateogoriesTestFixture _fixture;

        public ListCategoriesTest(ListCateogoriesTestFixture fixture) => _fixture = fixture;

        [Fact(DisplayName = nameof(List))]
        [Trait("Application", "ListCategories - UseCases")]
        public async Task List()
        {
            var categoriesExampleList = _fixture.GetExampleCategoriesList();
            var repositoryMock = _fixture.GetRepositoryMock();
            var input = _fixture.GetExampleInput();

            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>
            (
              currentPage: input.Page,
              perPage: input.PerPage,
              items: categoriesExampleList,
              total: new Random().Next(50, 200)
            );

            repositoryMock.Setup(
              repository => repository.Search(
                It.Is<SearchInput>(searchInput =>
                  searchInput.Page == input.Page &&
                  searchInput.PerPage == input.PerPage &&
                  searchInput.Search == input.Search &&
                  searchInput.OrderBy == input.Sort &&
                  searchInput.Order == input.Dir
                ),
                It.IsAny<CancellationToken>()
              )).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListCategories(repositoryMock.Object);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryCategory = outputRepositorySearch.Items
                    .FirstOrDefault(x => x.Id == outputItem.Id);
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryCategory!.Name);
                outputItem.Description.Should().Be(repositoryCategory.Description);
                outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
                outputItem.Id.Should().Be(repositoryCategory.Id);
                outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
            });

            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                  searchInput.Page == input.Page &&
                  searchInput.PerPage == input.PerPage &&
                  searchInput.Search == input.Search &&
                  searchInput.OrderBy == input.Sort &&
                  searchInput.Order == input.Dir
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ListOkWhenEmpty))]
        [Trait("Application", "ListCategories - UseCases")]
        public async Task ListOkWhenEmpty()
        {
            var categoriesExampleList = _fixture.GetExampleCategoriesList();
            var repositoryMock = _fixture.GetRepositoryMock();
            var input = _fixture.GetExampleInput();

            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>
            (
              currentPage: input.Page,
              perPage: input.PerPage,
              items: new List<DomainEntity.Category>().AsReadOnly(),
              total: 0
            );

            repositoryMock.Setup(
              repository => repository.Search(
                It.Is<SearchInput>(searchInput =>
                  searchInput.Page == input.Page &&
                  searchInput.PerPage == input.PerPage &&
                  searchInput.Search == input.Search &&
                  searchInput.OrderBy == input.Sort &&
                  searchInput.Order == input.Dir
                ),
                It.IsAny<CancellationToken>()
              )).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListCategories(repositoryMock.Object);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().HaveCount(0);

            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                  searchInput.Page == input.Page &&
                  searchInput.PerPage == input.PerPage &&
                  searchInput.Search == input.Search &&
                  searchInput.OrderBy == input.Sort &&
                  searchInput.Order == input.Dir
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(ListInputWithoutParameters))]
        [Trait("Application", "ListCategories - UseCases")]
        [MemberData(
            nameof(ListCategoriesTestDataGenerator.GetInputsWithoutAllParameters),
            parameters: 14,
            MemberType = typeof(ListCategoriesTestDataGenerator)
        )]
        public async Task ListInputWithoutParameters(UseCase.ListCategoriesInput input)
        {
            var categoriesExampleList = _fixture.GetExampleCategoriesList();
            var repositoryMock = _fixture.GetRepositoryMock();

            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>
            (
              currentPage: input.Page,
              perPage: input.PerPage,
              items: categoriesExampleList,
              total: new Random().Next(50, 200)
            );

            repositoryMock.Setup(
              repository => repository.Search(
                It.Is<SearchInput>(searchInput =>
                  searchInput.Page == input.Page &&
                  searchInput.PerPage == input.PerPage &&
                  searchInput.Search == input.Search &&
                  searchInput.OrderBy == input.Sort &&
                  searchInput.Order == input.Dir
                ),
                It.IsAny<CancellationToken>()
              )).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListCategories(repositoryMock.Object);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryCategory = outputRepositorySearch.Items
                    .FirstOrDefault(x => x.Id == outputItem.Id);
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryCategory!.Name);
                outputItem.Description.Should().Be(repositoryCategory.Description);
                outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
                outputItem.Id.Should().Be(repositoryCategory.Id);
                outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
            });

            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                  searchInput.Page == input.Page &&
                  searchInput.PerPage == input.PerPage &&
                  searchInput.Search == input.Search &&
                  searchInput.OrderBy == input.Sort &&
                  searchInput.Order == input.Dir
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
