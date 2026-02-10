using Bogus;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.EndToEndTests.Base;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common
{
    [CollectionDefinition(nameof(CastMemberAPIBaseFixture))]
    public class CastMemberBaseFixtureCollection : ICollectionFixture<CastMemberAPIBaseFixture> { }

    public class CastMemberAPIBaseFixture : BaseFixture
    {
        public CastMemberPersistence Persistence { get; set; }

        public CastMemberAPIBaseFixture() : base()
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

        public List<DomainEntity.CastMember> GetExampleCastMembersListByNames(List<string> names)
            => names.Select(name =>
            {
                var example = GetExampleCastMember();
                example.Update(name, example.Type);
                return example;
            }).ToList();

        public List<DomainEntity.CastMember> CloneCastMembersListOrdered(List<DomainEntity.CastMember> castMembersList, string orderBy, SearchOrder order)
        {
            var listClone = new List<DomainEntity.CastMember>(castMembersList);
            var orderedEnumerable = (orderBy.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.Id),
                ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase).ThenByDescending(x => x.Id),
                ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
                ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id),
                _ => listClone.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.Id)
            };
            return orderedEnumerable.ToList();
        }
    }
}
