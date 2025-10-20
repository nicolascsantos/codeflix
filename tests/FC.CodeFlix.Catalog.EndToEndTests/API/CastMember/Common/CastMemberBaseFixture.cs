using Bogus;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.EndToEndTests.Base;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common
{
    public class CastMemberBaseFixture : BaseFixture
    {
        public CastMemberPersistence Persistence { get; set; }

        public CastMemberBaseFixture() : base()
        {
            Persistence = new CastMemberPersistence(CreateDbContext());
        }

        public List<DomainEntity.CastMember> GetExampleCastMembersList(int length = 10) => Enumerable.Range(1, length).Select(
           _ => GetExampleCastMember())
                   .ToList();

        public DomainEntity.CastMember GetExampleCastMember()
          => new DomainEntity.CastMember(
              GetValidName(),
              GetRandomCastMemberType()
          );

        public CastMemberType GetRandomCastMemberType()
            => (CastMemberType)(new Random().Next(1, 2));

        public string GetValidName()
            => Faker.Name.FullName();
    }
}
