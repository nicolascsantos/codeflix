using FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models
{
    public class VideosGenres
    {
        public Genre? Genre { get; set; }

        public Video? Video { get; set; }

        public Guid VideoId { get; set; }

        public Guid GenreId { get; set; }

        public VideosGenres(Guid videoId, Guid genreId)
        {
            VideoId = videoId;
            GenreId = genreId;
        }
    }
}
