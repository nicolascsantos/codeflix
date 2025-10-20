using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMember
{
    [Collection(nameof(CastMemberUseCasesTestFixture))]
    public class ListCastMemberTest
    {
        private readonly CastMemberUseCasesTestFixture _fixture;

        public ListCastMemberTest(CastMemberUseCasesTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ListCastMember))]
        [Trait("Integration/Application", "ListCastMember - UseCases")]
        public async Task ListCastMember()
        {
            var examples = _fixture.GetCastMembersListExample(10);
            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(examples);
            await arrangeDbContext.SaveChangesAsync();
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            var useCase = new UseCase.ListCastMembers(repository);
            var input = new UseCase.ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().HaveCount(10);
            output.Total.Should().Be(examples.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.ToList().ForEach(outputItem =>
            {
                var exampleItem = examples.FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().BeEquivalentTo(outputItem);
            });
        }

        [Fact(DisplayName = nameof(EmptyList))]
        [Trait("Integration/Application", "ListCastMember - UseCases")]
        public async Task EmptyList()
        {
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext());
            var useCase = new UseCase.ListCastMembers(repository);
            var input = new UseCase.ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().HaveCount(0);
            output.Total.Should().Be(0);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
        }

        [Theory(DisplayName = nameof(ListCastMemberPaginatedList))]
        [Trait("Integration/Application", "ListCastMember - UseCases")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListCastMemberPaginatedList
        (
            int amountOfCastMembersToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems
        )
        {
            var examples = _fixture.GetCastMembersListExample(amountOfCastMembersToGenerate);
            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(examples);
            await arrangeDbContext.SaveChangesAsync();
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            var useCase = new UseCase.ListCastMembers(repository);
            var input = new UseCase.ListCastMembersInput(page, perPage, "", "", SearchOrder.Asc);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().HaveCount(expectedQuantityItems);
            output.Total.Should().Be(examples.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.ToList().ForEach(outputItem =>
            {
                var exampleItem = examples.FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().BeEquivalentTo(outputItem);
            });
        }

        [Theory(DisplayName = nameof(ListCastMemberPaginatedList))]
        [Trait("Integration/Application", "ListCastMember - UseCases")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task ListCastMemberSearchByText
        (
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems)
        {

            var exampleCastMembersList = _fixture.GetExampleCastMembersListByNames(new List<string>()
            {
                "Action",
                "Horror",
                "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Sci-fi AI",
                "Sci-fi Space",
                "Sci-fi Robots",
                "Sci-fi Future"
            });

            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(exampleCastMembersList);
            await arrangeDbContext.SaveChangesAsync();
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            var useCase = new UseCase.ListCastMembers(repository);
            var input = new UseCase.ListCastMembersInput(page, perPage, search, "", SearchOrder.Asc);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleCastMembersList.FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().BeEquivalentTo(outputItem);
            });
        }

        [Theory(DisplayName = nameof(ListCastMemberOrderedSearch))]
        [Trait("Integration/Application", "ListCastMember - UseCases")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdat", "asc")]
        [InlineData("createdat", "desc")]
        [InlineData("", "asc")]
        public async Task ListCastMemberOrderedSearch(string orderBy, string order)
        {
            var examples = _fixture.GetCastMembersListExample(10);
            var arrangeDbContext = _fixture.CreateDbContext();
            var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            await arrangeDbContext.AddRangeAsync(examples);
            await arrangeDbContext.SaveChangesAsync();
            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            var useCase = new UseCase.ListCastMembers(repository);
            var input = new UseCase.ListCastMembersInput(1, 20, "", orderBy, searchOrder);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().HaveCount(examples.Count);
            output.Total.Should().Be(examples.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);

            var orderedList = _fixture.CloneCategoriesListOrdered(examples, orderBy, searchOrder);
            for (int i = 0; i < orderedList.Count; i++)
                output.Items[i].Should().BeEquivalentTo(orderedList[i]);
        }
    }
}
