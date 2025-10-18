using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.UnitTests.Domain.Entity.CastMember;
using Moq;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.GetCastMember
{
    [CollectionDefinition(nameof(GetCastMemberTestFixture))]
    public class GetCastMemberTestFixtureCollection : ICollectionFixture<GetCastMemberTestFixture> { }
    public class GetCastMemberTestFixture : CastMemberTestFixture
    {
        public Mock<ICastMemberRepository> GetRepositoryMock() => new();
        public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
    }
}
