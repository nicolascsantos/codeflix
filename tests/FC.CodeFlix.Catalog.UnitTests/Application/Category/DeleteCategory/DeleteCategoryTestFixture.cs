using FC.CodeFlix.Catalog.UnitTests.Application.Category.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.DeleteCategory
{
    [CollectionDefinition(nameof(DeleteCategoryTestFixtureCollection))]
    public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture>
    {

    }

    public class DeleteCategoryTestFixture : CateoryUseCasesBaseFixture
    {

    }
}
