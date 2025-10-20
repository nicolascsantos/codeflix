using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [Fact(DisplayName = nameof(ThrowWhenCastMemberNotFound))]
        [Trait("EndToEnd/API", "CastMember/UpdateCastMember")]
        public async Task ThrowWhenCastMemberNotFound()
        {
            var castMemberExampleList = _fixture.GetExampleCastMembersList(10);
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(castMemberExampleList);

            var input = new UpdateCastMemberInput
            (
                randomGuid,
                _fixture.GetValidName(),
                _fixture.GetRandomCastMemberType()
            );
            var (response, output) = await _fixture.APIClient
                .Put<ProblemDetails>($"/api/CastMember/{randomGuid}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title = "One or more validation errors occurred.";
            output.Status = (int)StatusCodes.Status404NotFound;
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Cast member '{randomGuid}' not found."); ;
        }

        [Fact(DisplayName = nameof(Returns422IfThereAreValidationErrors))]
        [Trait("EndToEnd/API", "CastMember/UpdateCastMember")]
        public async Task Returns422IfThereAreValidationErrors()
        {
            var castMemberExampleList = _fixture.GetExampleCastMembersList(10);
            var castMemberToUpdate = castMemberExampleList[4];
            await _fixture.Persistence.InsertList(castMemberExampleList);

            var input = new UpdateCastMemberInput
            (
                castMemberToUpdate.Id,
                string.Empty,
                _fixture.GetRandomCastMemberType()
            );
            var (response, output) = await _fixture.APIClient
                .Put<ProblemDetails>($"/api/CastMember/{castMemberToUpdate.Id}", input);


            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output.Title.Should().Be("One or more validation errors occurred.");
            output.Type.Should().Be("UnprocessableEntity");
            output.Detail.Should().Be("Name should not be null or empty.");
            output.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        }
    }
}
