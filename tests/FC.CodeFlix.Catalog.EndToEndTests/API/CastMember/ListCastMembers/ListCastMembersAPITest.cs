using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common;
using FC.CodeFlix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.ListCastMembers
{
    [Collection(nameof(CastMemberAPIBaseFixture))]
    public class ListCastMembersAPITest : IDisposable
    {
        private readonly CastMemberAPIBaseFixture _fixture;

        public ListCastMembersAPITest(CastMemberAPIBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ListCastMembers))]
        [Trait("EndToEnd/API", "CastMember/ListCastMembers")]
        public async Task ListCastMembers()
        {
            var castMembersListExample = _fixture.GetExampleCastMembersList(10);
            await _fixture.Persistence.InsertList(castMembersListExample);

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CastMemberModelOutput>>("/api/CastMember");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.CurrentPage.Should().Be(1);
            output.Meta.Total.Should().Be(castMembersListExample.Count);
            output.Data.Should().HaveCount(castMembersListExample.Count);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = castMembersListExample.FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                exampleItem.Name.Should().Be(outputItem.Name);
                exampleItem.Type.Should().Be(outputItem.Type);
            });
        }

        [Fact(DisplayName = nameof(EmptyList))]
        [Trait("EndToEnd/API", "CastMember/ListCastMembers")]
        public async Task EmptyList()
        {
            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CastMemberModelOutput>>("/api/CastMember");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.CurrentPage.Should().Be(1);
            output.Meta.Total.Should().Be(0);
            output.Data.Should().HaveCount(0);
        }

        [Theory(DisplayName = nameof(PaginatedList))]
        [Trait("EndToEnd/API", "CastMember/ListCastMembers")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task PaginatedList(int amountOfCastMembersToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems)
        {
            var castMembersListExample = _fixture.GetExampleCastMembersList(amountOfCastMembersToGenerate);
            await _fixture.Persistence.InsertList(castMembersListExample);

            var input = new ListCastMembersInput(page, perPage, "", "", SearchOrder.Asc);

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CastMemberModelOutput>>("/api/CastMember", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.CurrentPage.Should().Be(page);
            output.Meta.PerPage.Should().Be(perPage);
            output.Meta.Total.Should().Be(castMembersListExample.Count);
            output.Data.Should().HaveCount(expectedQuantityItems);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = castMembersListExample.FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                exampleItem.Name.Should().Be(outputItem.Name);
                exampleItem.Type.Should().Be(outputItem.Type);
            });
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("EndToEnd/API", "CastMember/ListCastMembers")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
        )
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
            await _fixture.Persistence.InsertList(exampleCastMembersList);
            var input = new ListCastMembersInput(page, perPage, search, "", SearchOrder.Asc);

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CastMemberModelOutput>>("/api/CastMember", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.CurrentPage.Should().Be(page);
            output.Meta.PerPage.Should().Be(perPage);
            output.Meta.Total.Should().Be(expectedQuantityTotalItems);
            output.Data.Should().HaveCount(expectedQuantityItemsReturned);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleCastMembersList.FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                exampleItem.Name.Should().Be(outputItem.Name);
                exampleItem.Type.Should().Be(outputItem.Type);
            });
        }

        [Theory(DisplayName = nameof(OrderedSearch))]
        [Trait("EndToEnd/API", "CastMember/ListCastMembers")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdat", "asc")]
        [InlineData("createdat", "desc")]
        [InlineData("", "asc")]
        public async Task OrderedSearch(string orderBy, string order)
        {
            var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
            var castMemberListExample = _fixture.GetExampleCastMembersList(10);
            await _fixture.Persistence.InsertList(castMemberListExample);

            var input = new ListCastMembersInput(1, 10, "", orderBy, searchOrder);
            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<CastMemberModelOutput>>("/api/CastMember", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.CurrentPage.Should().Be(1);
            output.Meta.PerPage.Should().Be(10);
            output.Meta.Total.Should().Be(castMemberListExample.Count);
            output.Data.Should().HaveCount(castMemberListExample.Count);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = castMemberListExample.FirstOrDefault(x => x.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                exampleItem.Name.Should().Be(outputItem.Name);
                exampleItem.Type.Should().Be(outputItem.Type);
            });

            var orderedList = _fixture.CloneCastMembersListOrdered(castMemberListExample, orderBy, searchOrder);
            for (int i = 0; i < orderedList.Count; i++)
            {
                output.Data[i].Id.Should().Be(orderedList[i].Id);
                output.Data[i].Name.Should().Be(orderedList[i].Name);
                output.Data[i].Type.Should().Be(orderedList[i].Type);
                output.Data[i].CreatedAt.TrimMilliseconds().Should().Be(orderedList[i].CreatedAt.TrimMilliseconds());
            }
        }

        public void Dispose()
        {
            _fixture.CleanPersistence();
        }
    }
}
