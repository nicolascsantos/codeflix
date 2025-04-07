using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory
{
    [CollectionDefinition(nameof(CreateCategoryTestFixtureCollection))]
    public class CreateCategoryTestFixtureCollection { }

    public class CreateCategoryTestFixture : CategoryUseCasesBaseFixture
    {
        public CreateCategoryInput GetInput()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

        public CreateCategoryInput GetInvalidInputShortName()
        {
            var invalidInputShortName = GetInput();
            invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
            return invalidInputShortName;
        }

        public CreateCategoryInput GetInvalidInputTooLongName()
        {
            var invalidInputTooLongName = GetInput();
            var tooLongNameForCategory = Faker.Commerce.ProductName();
            while (tooLongNameForCategory.Length <= 255)
                tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";
            invalidInputTooLongName.Name = tooLongNameForCategory;
            return invalidInputTooLongName;
        }

        public CreateCategoryInput GetInvalidInputCategoryNull()
        {
            var invalidInputDescriptionNull = GetInput();
            invalidInputDescriptionNull.Description = null!;
            return invalidInputDescriptionNull;
        }

        public CreateCategoryInput GetInvalidInputTooLongDescription()
        {
            var invalidInputTooLongDescription = GetInput();
            var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
            while (tooLongDescriptionForCategory.Length <= 10_000)
                tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductDescription()}";
            invalidInputTooLongDescription.Description = tooLongDescriptionForCategory;
            return invalidInputTooLongDescription;
        }
    }
}
