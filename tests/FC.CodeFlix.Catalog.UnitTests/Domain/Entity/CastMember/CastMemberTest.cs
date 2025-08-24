using FluentAssertions;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.Exceptions;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.CastMember
{
    [Collection(nameof(CastMemberTestFixture))]
    public class CastMemberTest
    {
        private readonly CastMemberTestFixture _fixture;

        public CastMemberTest(CastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "CastMember - Aggregate")]
        public void Instantiate()
        {
            var dateTimeBefore = DateTime.Now.AddSeconds(-1);
            var name = _fixture.GetValidName();
            var type = _fixture.GetRandomCastMemberType();

            DomainEntity.CastMember castMember = new DomainEntity.CastMember(
                name,
                type
            );

            var dateTimeAfter = DateTime.Now.AddSeconds(1);

            castMember.Id.Should().NotBeEmpty();
            castMember.Name.Should().Be(name);
            castMember.Type.Should().Be(type);
            (castMember.CreatedAt >= dateTimeBefore).Should().BeTrue();
            (castMember.CreatedAt <= dateTimeAfter).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(ThrowErrorWhenNameIsInvalid))]
        [Trait("Domain", "CastMember - Aggregate")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void ThrowErrorWhenNameIsInvalid(string? invalidName)
        {
            var type = _fixture.GetRandomCastMemberType();

            var action = () => new DomainEntity.CastMember(
                invalidName!,
                type
            );

            action.Should().
                Throw<EntityValidationException>()
                .WithMessage("Name should not be empty or null.");
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "CastMember - Aggregate")]
        public void Update()
        {
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            var castMember = _fixture.GetExampleCastMember();

            castMember.Update(newName, newType);

            castMember.Id.Should().NotBeEmpty();
            castMember.Name.Should().Be(newName);
            castMember.Type.Should().Be(newType);
        }

        [Theory(DisplayName = nameof(UpdateThrowsWhenNameIsInvalid))]
        [Trait("Domain", "CastMember - Aggregate")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateThrowsWhenNameIsInvalid(string? invalidName)
        {
            var newType = _fixture.GetRandomCastMemberType();
            var castMember = _fixture.GetExampleCastMember();

            var action = () => castMember.Update(invalidName!, newType);

            action.Should().
                Throw<EntityValidationException>()
                .WithMessage("Name should not be empty or null.");
        }
    }
}
