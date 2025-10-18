using FC.CodeFlix.Catalog.UnitTests.Application.CastMember.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.GetCastMember
{
    [CollectionDefinition(nameof(GetCastMemberTestFixture))]
    public class GetCastMemberTestFixtureCollection : ICollectionFixture<GetCastMemberTestFixture> { }
    public class GetCastMemberTestFixture : CastMemberUseCaseFixture
    {
    }
}
