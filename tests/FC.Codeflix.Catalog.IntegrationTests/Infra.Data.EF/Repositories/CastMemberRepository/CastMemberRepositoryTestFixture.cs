using FC.Codeflix.Catalog.IntegrationTests.Base;
using FC.CodeFlix.Catalog.Domain.Enum;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository
{
    [CollectionDefinition(nameof(CastMemberRepositoryTestFixture))]
    public class CastMemberRepositoryTestFixtureCollection : ICollectionFixture<CastMemberRepositoryTestFixture> { }

    public class CastMemberRepositoryTestFixture : BaseFixture
    {
        public List<DomainEntity.CastMember> GetCastMembersListExample(int quantity)
            => Enumerable.Range(1, quantity).Select(_ => GetExampleCastMember()).ToList();

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
