using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.UpdateCastMember
{
    [Collection(nameof(CastMemberAPIBaseFixture))]
    public class UpdateCastMemberAPITest
    {
        private readonly CastMemberAPIBaseFixture _fixture;

        public UpdateCastMemberAPITest(CastMemberAPIBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateCastMember))]
        [Trait("EndToEnd/API", "CastMember/UpdateCastMember")]
        public async Task UpdateCastMember()
        {
            var castMemberExampleList = _fixture.GetExampleCastMembersList(10);
            var castMemberToUpdate = castMemberExampleList[4];
            await _fixture.Persistence.InsertList(castMemberExampleList);

            var input = new UpdateCastMemberInput
            (
                castMemberToUpdate.Id,
                _fixture.GetValidName(),
                _fixture.GetRandomCastMemberType()
            );
            var (response, output) = await _fixture.APIClient
                .Put<APIResponse<CastMemberModelOutput>>($"/api/CastMember/{castMemberToUpdate.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Data.Id.Should().Be(castMemberToUpdate.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Type.Should().Be(input.Type);
            output.Data.CreatedAt.Should().NotBeSameDateAs(default);
            var castMemberFromDb = await _fixture.Persistence.GetById(castMemberToUpdate.Id);
            castMemberFromDb.Should().NotBeNull();
            castMemberFromDb.Name.Should().Be(input.Name);
            castMemberFromDb.Type.Should().Be(input.Type);
        }
    }
}
