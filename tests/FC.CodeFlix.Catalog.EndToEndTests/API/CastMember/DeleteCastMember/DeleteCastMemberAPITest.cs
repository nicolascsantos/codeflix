using FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.DeleteCastMember
{
    [Collection(nameof(CastMemberAPIBaseFixture))]
    public class DeleteCastMemberAPITest
    {
        private readonly CastMemberAPIBaseFixture _fixture;

        public DeleteCastMemberAPITest(CastMemberAPIBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCastMember))]
        [Trait("EndToEnd/API", "CastMember/DeleteCastMember")]
        public async Task DeleteCastMember()
        {
            var examples = _fixture.GetExampleCastMembersList(10);
            var castMemberExampleFromList = examples[2];
            await _fixture.Persistence.InsertList(examples);

            var (response, output) = await _fixture.APIClient
                .Delete<object>($"/api/CastMember/{castMemberExampleFromList.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();
            var persistenceCategory = await _fixture.Persistence.GetById(castMemberExampleFromList.Id);
            persistenceCategory.Should().BeNull();
        }

        [Fact(DisplayName = nameof(ThrowsWhenCastMemberNotFound))]
        [Trait("EndToEnd/API", "CastMember/DeleteCastMember")]
        public async Task ThrowsWhenCastMemberNotFound()
        {
            var examples = _fixture.GetExampleCastMembersList(10);
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(examples);

            var (response, output) = await _fixture.APIClient
                .Delete<ProblemDetails>($"/api/CastMember/{randomGuid}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title = "One or more validation errors occurred.";
            output.Status = (int)StatusCodes.Status404NotFound;
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Cast member '{randomGuid}' not found."); ;   
        }
    }
}
