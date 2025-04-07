using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory
{
    [CollectionDefinition(nameof(CategoryUseCasesBaseFixture))]
    public class UpdateCategoryTestFixtureCollection { }

    public class UpdateCategoryTestFixture : CategoryUseCasesBaseFixture
    {
        public DomainEntity.Category GetValidCategory()
            => new(GetValidCategoryName(),
                GetValidCategoryDescription()
        );

        public UpdateCategoryInput GetValidInput(Guid? id = null) => new
        (
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

        public UpdateCategoryInput GetInvalidInputShortName()
        {
            var invalidInputShortName = GetValidInput();
            invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
            return invalidInputShortName;
        }

        public UpdateCategoryInput GetInvalidInputTooLongName()
        {
            var invalidInputTooLongName = GetValidInput();
            var tooLongNameForCategory = Faker.Commerce.ProductName();
            while (tooLongNameForCategory.Length <= 255)
                tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";
            invalidInputTooLongName.Name = tooLongNameForCategory;
            return invalidInputTooLongName;
        }

        public UpdateCategoryInput GetInvalidInputTooLongDescription()
        {
            var invalidInputTooLongDescription = GetValidInput();
            var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
            while (tooLongDescriptionForCategory.Length <= 10_000)
                tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductDescription()}";
            invalidInputTooLongDescription.Description = tooLongDescriptionForCategory;
            return invalidInputTooLongDescription;
        }
    }
}
