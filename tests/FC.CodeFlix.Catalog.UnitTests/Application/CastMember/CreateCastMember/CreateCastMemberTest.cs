using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.CreateCastMember
{
    [Collection(nameof(CreateCastMemberTestFixture))]
    public class CreateCastMemberTest
    {
        private readonly CreateCastMemberTestFixture _fixture;

        public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateCastMember))]
        [Trait("Application", "CreateCastMember - Use Cases")]
        public async Task CreateCastMember()
        {
            var input = new CreateCastMemberInput
            (
                _fixture.GetValidName(),
                _fixture.GetRandomCastMemberType()
            );

            var repositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UseCases.CreateCastMember
            (
                repositoryMock.Object,
                unitOfWorkMock.Object
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Name.Should().Be(input.Name);
            output.Type.Should().Be(input.Type);
            output.CreatedAt.Should().NotBeSameDateAs(default);
            unitOfWorkMock.Verify
            (
                x => x.Commit(It.IsAny<CancellationToken>()),
                Times.Once
            );
            repositoryMock.Verify
            (
                x => x.Insert(It.Is<DomainEntity.CastMember>(x => x.Name == input.Name && x.Type == input.Type), It.IsAny<CancellationToken>()), Times.Once
            );
        }

        [Theory(DisplayName = nameof(ThrowsWhenInvalidName))]
        [Trait("Application", "CreateCastMember - Use Cases")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public async Task ThrowsWhenInvalidName(string? invalidName)
        {
            var input = new CreateCastMemberInput
           (
               invalidName!,
               _fixture.GetRandomCastMemberType()
           );

            var repositoryMock = new Mock<ICastMemberRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var useCase = new UseCases.CreateCastMember
           (
               repositoryMock.Object,
               unitOfWorkMock.Object
           );

            var action = async () => await useCase.Handle(input, CancellationToken.None);
            await action.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage("Name should not be empty or null.");
        }
    }
}
