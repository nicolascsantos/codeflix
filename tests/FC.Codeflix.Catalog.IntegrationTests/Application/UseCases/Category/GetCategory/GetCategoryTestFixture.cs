using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory.Common;
using FC.Codeflix.Catalog.IntegrationTests.Base;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory
{
    [Collection(nameof(CategoryUseCasesBaseFixture))]
    public class GetCategoryTestFixtureCollection : ICollectionFixture<CategoryUseCasesBaseFixture> { }
    public class GetCategoryTestFixture : CategoryUseCasesBaseFixture
    {

    }
}
