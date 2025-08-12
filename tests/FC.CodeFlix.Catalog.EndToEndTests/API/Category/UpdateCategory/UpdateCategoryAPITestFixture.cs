using Bogus;
using FC.CodeFlix.Catalog.API.APIModels.Category;
using FC.CodeFlix.Catalog.EndToEndTests.API.Category.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.UpdateCategory
{
    [CollectionDefinition(nameof(UpdateCategoryAPITestFixture))]
    public class UpdateCategoryAPITestFixtureCollection : ICollectionFixture<UpdateCategoryAPITestFixture> { }

    public class UpdateCategoryAPITestFixture : CategoryBaseFixture
    {
        public UpdateCategoryAPIInput GetExampleInput()
        {
            return new UpdateCategoryAPIInput(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );
        }

        public string GetInvalidNameTooShort()
        {
            return GetValidCategoryName().Substring(0, 2);
        }

        public string GetInvalidNameTooLong()
        {
            string validName = GetValidCategoryName();
            string longName = "";
            while (longName.Length <= 255)
                longName += $"{validName} {Faker.Commerce.ProductName()}";
            return longName;
        }

        public string GetInvalidDescriptionTooLong()
        {
            string validDescription = GetValidCategoryDescription();
            string tooLongDescription = "";
            while (tooLongDescription.Length <= 10_000)
                tooLongDescription += $"{validDescription} {Faker.Commerce.ProductDescription()}";
            return tooLongDescription;
        }
    }
}
