using FC.CodeFlix.Catalog.UnitTests.Common;

namespace FC.CodeFlix.Catalog.UnitTests.Infra.Storage
{
    [CollectionDefinition(nameof(StorageServiceTestFixture))]
    public class StorageServiceTestFixtureCollection : ICollectionFixture<StorageServiceTestFixture> { }

    public class StorageServiceTestFixture : BaseFixture
    {
        public string GetBucketName() => "fc3-catalog-medias";

        public string GetFileName() => Faker.System.CommonFileName();

        public string GetFileContent() => Faker.Lorem.Paragraph();

        public string GetContentType() => Faker.System.MimeType();
    }
}
