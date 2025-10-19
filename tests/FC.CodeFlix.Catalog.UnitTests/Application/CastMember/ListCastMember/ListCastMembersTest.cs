using FC.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;


namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.ListCastMember
{
    [Collection(nameof(ListCastMembersTestFixture))]
    public class ListCastMembersTest
    {
        private readonly ListCastMembersTestFixture _fixture;

        public ListCastMembersTest(ListCastMembersTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ListCastMembers))]
        [Trait("Application", "ListCastMembers - Use Cases")]
        public async Task ListCastMembers()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var castMembersListExample = _fixture.GetCastMembersListExample(3);
            var repositorySearchOutput = new SearchOutput<DomainEntity.CastMember>
            (
                1,
                10,
                castMembersListExample.Count,
                castMembersListExample.AsReadOnly()
            );

            repositoryMock.Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>())).ReturnsAsync(repositorySearchOutput);

            var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);
            var useCase = new ListCastMembers(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(repositorySearchOutput.CurrentPage);
            output.PerPage.Should().Be(repositorySearchOutput.PerPage);
            output.Total.Should().Be(repositorySearchOutput.Items.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                var example = castMembersListExample.Find(x => x.Id == outputItem.Id);
                example.Should().NotBeNull();
                example.Name.Should().Be(example.Name);
                example.Type.Should().Be(example.Type);
            });

            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(x => (
                x.Page == input.Page &&
                x.PerPage == input.PerPage &&
                x.Search == input.Search &&
                x.Order == input.Dir &&
                x.OrderBy == input.Sort)), It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = nameof(ReturnsEmptyWhenIsEmpty))]
        [Trait("Application", "ListCastMembers - Use Cases")]
        public async Task ReturnsEmptyWhenIsEmpty()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var castMembersListExample = new List<DomainEntity.CastMember>();
            var repositorySearchOutput = new SearchOutput<DomainEntity.CastMember>
            (
                1,
                10,
                castMembersListExample.Count,
                castMembersListExample.AsReadOnly()
            );

            repositoryMock.Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>())).ReturnsAsync(repositorySearchOutput);

            var input = new ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);
            var useCase = new ListCastMembers(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(repositorySearchOutput.CurrentPage);
            output.PerPage.Should().Be(repositorySearchOutput.PerPage);
            output.Total.Should().Be(repositorySearchOutput.Items.Count);
            output.Items.Should().HaveCount(0);

            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(x => (
                x.Page == input.Page &&
                x.PerPage == input.PerPage &&
                x.Search == input.Search &&
                x.Order == input.Dir &&
                x.OrderBy == input.Sort)), It.IsAny<CancellationToken>()));
        }
    }
}
