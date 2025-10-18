using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using Moq;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.Common
{
    [CollectionDefinition(nameof(CastMemberUseCaseFixture))]
    public class CastMemberUseCaseFixtureCollection : ICollectionFixture<CastMemberUseCaseFixture> { }

    public class CastMemberUseCaseFixture
    {
        public Mock<ICastMemberRepository> GetRepositoryMock() => new();

        public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

        public string GetValidCastMemberName()
        {
            throw new NotImplementedException();
        }
    }
}
