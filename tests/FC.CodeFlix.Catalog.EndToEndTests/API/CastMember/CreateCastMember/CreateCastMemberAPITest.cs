using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.CreateCastMember
{
    [Collection(nameof(CastMemberAPIBaseFixture))]
    public class CreateCastMemberAPITest
    {
        private readonly CastMemberAPIBaseFixture _fixture;

        public CreateCastMemberAPITest(CastMemberAPIBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateCastMember))]
        [Trait("EndToEnd/API", "CastMember/CreateCastMember")]
        public async Task CreateCastMember()
        {
            var validCastMember = _fixture.GetExampleCastMember();
            var input = new CreateCastMemberInput(validCastMember.Name, validCastMember.Type);

            var (response, output) = await _fixture.APIClient
                .Post<APIResponse<CastMemberModelOutput>>("/api/CastMember", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
            output.Should().NotBeNull();
            output.Data.Name.Should().Be(validCastMember.Name);
            output.Data.Type.Should().Be(validCastMember.Type);

            var persistenceCastMember = await _fixture.Persistence.GetById(output.Data.Id);
            persistenceCastMember.Should().NotBeNull();
            persistenceCastMember.Id.Should().NotBeEmpty();
            persistenceCastMember.Name.Should().Be(validCastMember.Name);
            persistenceCastMember.Type.Should().Be(validCastMember.Type);
        }

        [Fact(DisplayName = nameof(ThrowWhenNameIsEmpty))]
        [Trait("EndToEnd/API", "CastMember/CreateCastMember")]
        public async Task ThrowWhenNameIsEmpty()
        {
            var validCastMember = _fixture.GetExampleCastMember();
            var input = new CreateCastMemberInput(string.Empty, validCastMember.Type);

            var (response, output) = await _fixture.APIClient
                .Post<ProblemDetails>("/api/CastMember", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            output.Should().NotBeNull();
            output.Title.Should().Be("One or more validation errors occurred.");
            output.Type.Should().Be("UnprocessableEntity");
            output.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
            output.Detail.Should().Be("Name should not be empty or null.");
        }
    }
}
