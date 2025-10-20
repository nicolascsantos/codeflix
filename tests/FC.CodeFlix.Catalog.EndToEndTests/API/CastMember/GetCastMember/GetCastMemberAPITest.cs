using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.GetCastMember
{
    [Collection(nameof(CastMemberAPIBaseFixture))]
    public class GetCastMemberAPITest
    {
        private readonly CastMemberAPIBaseFixture _fixture;

        public GetCastMemberAPITest(CastMemberAPIBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(GetCastMember))]
        [Trait("EndToEnd/API", "CastMember/GetCastMemberById")]
        public async Task GetCastMember() 
        {
            var examples = _fixture.GetExampleCastMembersList(10);
            var castMemberExampleFromList = examples[2];
            await _fixture.Persistence.InsertList(examples);

            var  (response, output) = await _fixture.APIClient
                .Get<APIResponse<CastMemberModelOutput>>($"/api/CastMember/{castMemberExampleFromList.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(castMemberExampleFromList.Id);
            output.Data.Name.Should().Be(castMemberExampleFromList.Name);
            output.Data.Type.Should().Be(castMemberExampleFromList.Type);
        }

        [Fact(DisplayName = nameof(ThrowWhenCastMemberNotFound))]
        [Trait("EndToEnd/API", "CastMember/GetCastMemberById")]
        public async Task ThrowWhenCastMemberNotFound()
        {
            var examples = _fixture.GetExampleCastMembersList(10);
            await _fixture.Persistence.InsertList(examples);
            var randomGuid = Guid.NewGuid();

            var (response, output) = await _fixture.APIClient
                .Get<ProblemDetails>($"/api/CastMember/{randomGuid}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title = "One or more validation errors occurred.";
            output.Status = (int)StatusCodes.Status404NotFound;
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Cast member '{randomGuid}' not found.");

        }
    }
}
