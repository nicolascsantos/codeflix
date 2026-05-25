using FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common
{
    public class VideoBaseFixture : GenreBaseFixture
    {
        public VideoPersistence VideoPersistence { get; set; }

        public VideoBaseFixture() : base()
        {
            VideoPersistence = new VideoPersistence(_dbContext);
        }
    }
}
