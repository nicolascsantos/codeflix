using FC.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.Extensions;

namespace FC.CodeFlix.Catalog.API.APIModels.Video
{
    public class UpdateVideoAPIInput
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int YearLaunched { get; set; }

        public bool Opened { get; set; }

        public bool Published { get; set; }

        public int Duration { get; set; }

        public string? Rating { get; set; }

        public List<Guid>? CategoriesIds { get; set; }

        public List<Guid>? GenresIds { get; set; }

        public List<Guid>? CastMembersIds { get; set; }

        public UpdateVideoInput ToInput(Guid id)
            => new UpdateVideoInput(
                id,
                Title,
                Description,
                YearLaunched,
                Opened,
                Published,
                Duration,
                Rating.ToRating(),
                GenresIds,
                CategoriesIds,
                CastMembersIds
            );
    }
}
