using Bogus;
using FC.CodeFlix.Catalog.EndToEndTests.Base;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.Common
{
    public class CategoryBaseFixture : BaseFixture
    {
        public CategoryPersistence Persistence { get; set; }

        public CategoryBaseFixture() : base()
        {
            Persistence = new CategoryPersistence(CreateDbContext());
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

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

        public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10) => Enumerable.Range(1, length).Select(
           _ => GetExampleCategory())
                   .ToList();

        public DomainEntity.Category GetExampleCategory()
        {
            return new DomainEntity.Category(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );
        }
    }
}
