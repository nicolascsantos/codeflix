using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.CodeFlix.Catalog.UnitTests.Application.Category.Common;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesTestFixtureCollection))]
    public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCateogoriesTestFixture>
    { }

    public class ListCateogoriesTestFixture : CateoryUseCasesBaseFixture
    {

        public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10)
        {
            var list = new List<DomainEntity.Category>();
            for (int i = 0; i < length; i++)
                list.Add(GetExampleCategory());
            return list;
        }

        public ListCategoriesInput GetExampleInput()
        {
            var random = new Random();
            return new ListCategoriesInput(
              page: random.Next(1, 10),
              perPage: random.Next(15, 100),
              search: Faker.Commerce.ProductName(),
              sort: Faker.Commerce.ProductName(),
              dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
            );
        }
    }
}
