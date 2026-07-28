using FC.CodeFlix.Catalog.Infra.Storage.Enum;

namespace FC.CodeFlix.Catalog.Infra.Storage.Configuration
{
    public class StorageServiceOptions
    {
        public const string ConfigurationSection = "Storage";

        public StorageServiceOptions(string bucketName)
        {
            BucketName = bucketName;
        }

        public StorageServiceOptions() {}

        public string BucketName { get; set; }

        public StorageProviderType StorageProvider { get; set; } = StorageProviderType.AWS;
    }
}
