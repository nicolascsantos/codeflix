namespace FC.CodeFlix.Catalog.Application.Common
{
    public static class StorageName
    {
        public static string Create(Guid mediaId, string propertyName, string fileExtension)
           => $"{mediaId}-{propertyName}.{fileExtension}";
    }
}
