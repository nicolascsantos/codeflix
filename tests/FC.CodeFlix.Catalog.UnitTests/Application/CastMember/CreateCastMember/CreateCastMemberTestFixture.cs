using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.UnitTests.Domain.Entity.CastMember;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.CreateCastMember
{
    [CollectionDefinition(nameof(CreateCastMemberTestFixture))]
    public class CreateCastMemberTestFixtureCollection : IClassFixture<CreateCastMemberTestFixture> { }
        
    public class CreateCastMemberTestFixture : CastMemberTestFixture
    {
    }
}
