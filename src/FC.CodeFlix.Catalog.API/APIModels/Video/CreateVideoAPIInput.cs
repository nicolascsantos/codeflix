using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.CodeFlix.Catalog.Domain.Enum;
using MediatR;

namespace FC.CodeFlix.Catalog.API.APIModels.Video
{
    public class CreateVideoAPIInput : IRequest<VideoModelOutput>
    {
        public CreateVideoAPIInput(
            string title,
            string description,
            int yearLaunched,
            bool opened,
            bool published,
            int duration,
            Rating rating,
            List<Guid>? categoriesIds,
            List<Guid>? genresIds,
            List<Guid>? castMembersIds
        )
        {
            Title = title;
            Description = description;
            YearLaunched = yearLaunched;
            Opened = opened;
            Published = published;
            Duration = duration;
            Rating = rating;
            CategoriesIds = categoriesIds;
            GenresIds = genresIds;
            CastMembersIds = castMembersIds;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public int YearLaunched { get; set; }

        public bool Opened { get; set; }

        public bool Published { get; set; }

        public int Duration { get; set; }

        public Rating Rating { get; set; }

        public List<Guid>? CategoriesIds { get; set; }

        public List<Guid>? GenresIds { get; set; }

        public List<Guid>? CastMembersIds { get; set; }

        public CreateVideoInput ToCreateVideoInput()
            => new CreateVideoInput(
                Title,
                Description,
                YearLaunched,
                Opened,
                Published,
                Duration,
                Rating,
                CategoriesIds?.AsReadOnly(),
                GenresIds?.AsReadOnly(),
                CastMembersIds?.AsReadOnly()
            );
    }
}
