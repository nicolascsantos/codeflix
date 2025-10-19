using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.UpdateCastMember
{
    [Collection(nameof(UpdateCastMemberTestFixture))]
    public class UpdateCastMemberTest
    {
        private readonly UpdateCastMemberTestFixture _fixture;

        public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateCastMember))]
        [Trait("Application", "UpdateCastMember - Use Cases")]
        public async Task UpdateCastMember()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var validCastMember = _fixture.GetExampleCastMember();
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(validCastMember);
            var input = new UpdateCastMemberInput(
                validCastMember.Id,
                newName,
                newType
            );
            var useCase = new UseCases.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

            CastMemberModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Id.Should().Be(validCastMember.Id);
            output.Name.Should().Be(newName);
            output.Type.Should().Be(newType);

            repositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == input.Id), It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.Update(validCastMember, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "UpdateCastMember - Use Cases")]
        public async Task ThrowWhenNotFound()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            var randomGuid = Guid.NewGuid();
            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Cast member '{randomGuid}' not found."));
            var input = new UpdateCastMemberInput(
                randomGuid,
                newName,
                newType
            );
            var useCase = new UseCases.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

            var action = async () =>  await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Cast member '{randomGuid}' not found.");
        }



        [Theory(DisplayName = nameof(ThrowWhenNotFound))]
        [Trait("Application", "UpdateCastMember - Use Cases")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public async Task ThrowWhenNameIsInvalid(string? invalidName)
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var validCastMember = _fixture.GetExampleCastMember();
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            repositoryMock.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(validCastMember);
            var input = new UpdateCastMemberInput(
                validCastMember.Id,
                invalidName!,
                newType
            );
            var useCase = new UseCases.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage("Name should not be empty or null.");
        }
    }
}
