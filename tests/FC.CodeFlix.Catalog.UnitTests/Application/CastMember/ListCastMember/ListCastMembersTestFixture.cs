using FC.CodeFlix.Catalog.UnitTests.Application.CastMember.Common;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.ListCastMember
{
    [CollectionDefinition(nameof(ListCastMembersTestFixture))]
    public class ListCastMembersTestFixtureCollection : ICollectionFixture<ListCastMembersTestFixture> { }
    public class ListCastMembersTestFixture : CastMemberUseCaseFixture
    {
        public List<DomainEntity.CastMember> GetCastMembersListExample(int quantity)
            => Enumerable.Range(1, quantity).Select(_ => GetExampleCastMember()).ToList();
    }
}
