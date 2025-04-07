using FC.CodeFlix.Catalog.EndToEndTests.API.Category.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.DeleteCategory
{
    [CollectionDefinition(nameof(DeleteCategoryAPITestFixture))]
    public class DeleteCategoryAPITestFixureCollection : ICollectionFixture<DeleteCategoryAPITestFixture> { }

    public class DeleteCategoryAPITestFixture : CategoryBaseFixture
    {
    }
}
