using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FluentAssertions;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.GetCastMember
{
    [Collection(nameof(GetCastMemberTestFixture))]
    public class GetCastMemberTest
    {
        private readonly GetCastMemberTestFixture _fixture;

        public GetCastMemberTest(GetCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(GetCastMember))]
        [Trait("Application", "GetCastMember - Use Cases")]
        public async Task GetCastMember()
        {
            var validCastMember = _fixture.GetExampleCastMember();
            var repositoryMock = _fixture.GetRepositoryMock();
            var input = new GetCastMemberInput(validCastMember.Id);
            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(validCastMember);
            var useCase = new UseCases.GetCastMember
            (
                repositoryMock.Object
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(validCastMember.Name);
            output.Type.Should().Be(validCastMember.Type);
            output.CreatedAt.Should().NotBeSameDateAs(default);
            repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == validCastMember.Id), It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "GetCastMember - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var validCastMember = _fixture.GetExampleCastMember();
            var repositoryMock = _fixture.GetRepositoryMock();
            var input = new GetCastMemberInput(validCastMember.Id);
            repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException($"Cast member '{validCastMember.Id}' not found."));
            var useCase = new UseCases.GetCastMember
            (
                repositoryMock.Object
            );

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Cast member '{validCastMember.Id}' not found.");
        }
    }
}
