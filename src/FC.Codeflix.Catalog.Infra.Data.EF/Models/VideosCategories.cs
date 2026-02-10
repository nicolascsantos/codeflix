using FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models
{
    public class VideosCategories
    {
        public VideosCategories(Guid videoId, Guid categoryId)
        {
            VideoId = videoId;
            CategoryId = categoryId;
        }

        public Guid VideoId { get; set; }

        public Guid CategoryId { get; set; }

        public Category? Category { get; set; }

        public Video? Video { get; set; }
    }
}
