using FC.Codeflix.Catalog.IntegrationTests.Base;
using FC.CodeFlix.Catalog.Domain.Entity;
using Moq;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork
{
    [CollectionDefinition(nameof(UnitOfWorkTestFixtureCollection))]
    public class UnitOfWorkTestFixtureCollection : ICollectionFixture<UnitOfWorkTestFixture>
    {}

    public class UnitOfWorkTestFixture : BaseFixture
    {
        public Category GetExampleCategory()
        {
            return new Category(
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

        public List<Category> GetExampleCategoriesList(int length = 10) => Enumerable.Range(1, length).Select(
            _ => GetExampleCategory())
                    .ToList();
        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

    }
}
