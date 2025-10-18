using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.UnitTests.Common;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.CastMember
{
    [CollectionDefinition(nameof(CastMemberTestFixture))]
    public class CastMemberTestFixtureCollection : ICollectionFixture<CastMemberTestFixture> { }

    public class CastMemberTestFixture : BaseFixture
    {
        public DomainEntity.CastMember GetExampleCastMember()
            => new DomainEntity.CastMember(
                GetValidName(),
                GetRandomCastMemberType()
            );

        public string GetValidName()
            => Faker.Name.FullName();

        public CastMemberType GetRandomCastMemberType()
            => (CastMemberType)(new Random().Next(1, 2));
    }
}
