using FC.Codeflix.Catalog.IntegrationTests.Base;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory.Common
{
    public class CategoryUseCasesBaseFixture : BaseFixture
    {
        public DomainEntity.Category GetExampleCategory()
        {
            return new DomainEntity.Category(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );
        }

        public string GetValidCategoryName()
        {
            var categoryName = "";
            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];
            if (categoryName.Length > 255)
                categoryName = categoryName[..255];
            return categoryName;
        }

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            while (categoryDescription.Length > 10_000)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }

        public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10) => Enumerable.Range(1, length).Select(
            _ => GetExampleCategory())
                    .ToList();

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

    }
}
