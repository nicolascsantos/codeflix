using FC.CodeFlix.Catalog.Domain.Enum;

namespace FC.CodeFlix.Catalog.Domain.Entity
{
    public class Media
    {

        public string FilePath { get; private set; }

        public string? EncodedPath { get; private set; }

        public MediaStatus Status { get; private set; }

        public Media(string filePath)
        {
            FilePath = filePath;
            Status = MediaStatus.PENDING;
        }

        public void UpdateAsSentToEncode()
            => Status = MediaStatus.PROCESSING;

        public void UpdateAsEncoded(string encodedExamplePath)
        {
            Status = MediaStatus.COMPLETED;
            EncodedPath = encodedExamplePath;
        }
    }
}
