using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FluentAssertions;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.DeleteCastMember
{
    [Collection(nameof(DeleteCastMemberTestFixture))]
    public class DeleteCastMemberTest
    {
        private readonly DeleteCastMemberTestFixture _fixture;

        public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCastMember))]
        [Trait("Application", "DeleteCastMember - Use Cases")]
        public async Task DeleteCastMember()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var validCastMember = _fixture.GetExampleCastMember();

            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(validCastMember);

            var input = new UseCases.DeleteCastMemberInput(validCastMember.Id);
            var useCase = new UseCases.DeleteCastMember(repositoryMock.Object, unitOfWorkMock.Object);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().NotThrowAsync();

            repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id), It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.Delete(validCastMember, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "DeleteCastMember - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var randomGuid = Guid.NewGuid();
            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Cast member '{randomGuid}' not found."));
            var input = new DeleteCastMemberInput(randomGuid);
            var useCase = new UseCases.DeleteCastMember(repositoryMock.Object, unitOfWorkMock.Object);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Cast member '{randomGuid}' not found.");
        }
    }
}
