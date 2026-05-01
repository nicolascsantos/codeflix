namespace FC.CodeFlix.Catalog.Infra.Storage.Configuration
{
    public class StorageServiceOptions
    {
        public const string ConfigurationSection = "Storage";

        public StorageServiceOptions(string bucketName)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; set; }
    }
}
