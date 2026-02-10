namespace FC.CodeFlix.Catalog.Infra.Storage.Configuration
{
    public class StorageServiceOptions
    {
        public StorageServiceOptions(string bucketName)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; set; }
    }
}
