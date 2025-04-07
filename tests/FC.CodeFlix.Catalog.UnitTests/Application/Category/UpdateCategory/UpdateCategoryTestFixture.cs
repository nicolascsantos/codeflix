using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.CodeFlix.Catalog.UnitTests.Application.Category.Common;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.UpdateCategory
{
    [CollectionDefinition(nameof(UpdateCategoryTestFixtureCollection))]
    public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture>
    {

    }

    public class UpdateCategoryTestFixture : CateoryUseCasesBaseFixture
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

        public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10)
        {
            var list = new List<DomainEntity.Category>();
            for (int i = 0; i < length; i++)
                list.Add(GetValidCategory());
            return list;
        }
    }
}
