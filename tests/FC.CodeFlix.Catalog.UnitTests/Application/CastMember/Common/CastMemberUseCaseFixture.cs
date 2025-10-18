using Bogus;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.UnitTests.Common;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.CastMember.Common
{
    [CollectionDefinition(nameof(CastMemberUseCaseFixture))]
    public class CastMemberUseCaseFixtureCollection : ICollectionFixture<CastMemberUseCaseFixture> { }

    public class CastMemberUseCaseFixture : BaseFixture
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

        public Mock<ICastMemberRepository> GetRepositoryMock() => new();

        public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

        public string GetValidCastMemberName()
        {
            throw new NotImplementedException();
        }
    }
}
