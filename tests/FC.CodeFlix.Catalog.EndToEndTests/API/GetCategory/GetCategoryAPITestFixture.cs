using FC.CodeFlix.Catalog.EndToEndTests.API.Category.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.GetCategory
{
    [CollectionDefinition(nameof(GetCategoryAPITestFixture))]
    public class GetCategoryAPITestFixtureCollection : ICollectionFixture<GetCategoryAPITestFixture> { }

    public class GetCategoryAPITestFixture : CategoryBaseFixture
    {

    }
}
