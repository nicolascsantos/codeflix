using FC.Codeflix.Catalog.IntegrationTests.Base;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common
{
    public class CastMemberUseCasesTestFixture : BaseFixture
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

        public List<DomainEntity.CastMember> GetExampleCastMembersListByNames(List<string> names)
            => names.Select(name =>
            {
                var example = GetExampleCastMember();
                example.Update(name, example.Type);
                return example;
            }).ToList();

        public List<DomainEntity.CastMember> CloneCategoriesListOrdered(List<DomainEntity.CastMember> castMembersList, string orderBy, SearchOrder order)
        {
            var listClone = new List<DomainEntity.CastMember>(castMembersList);
            var orderedEnumerable = (orderBy.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),
                ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
                ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
                _ => listClone.OrderBy(x => x.Name)
            };
            return orderedEnumerable.ToList();
        }
    }
}
