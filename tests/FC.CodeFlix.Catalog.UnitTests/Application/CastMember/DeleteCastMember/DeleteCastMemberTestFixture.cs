using FC.CodeFlix.Catalog.UnitTests.Application.CastMember.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.DeleteCastMember
{
    [CollectionDefinition(nameof(DeleteCastMemberTestFixture))]
    public class DeleteCastMemberTestFixtureCollection : ICollectionFixture<DeleteCastMemberTestFixture> { }

    public class DeleteCastMemberTestFixture : CastMemberUseCaseFixture
    {
    }
}
