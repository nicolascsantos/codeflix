using FC.CodeFlix.Catalog.Domain.Extensions;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.Common
{
    public record VideoModelOutput(
        Guid Id,
        string Title,
        string Description,
        int YearLaunched,
        bool Opened,
        bool Published,
        int Duration,
        string Rating,
        DateTime CreatedAt,
        IReadOnlyCollection<Guid> CategoriesIds,
        IReadOnlyCollection<Guid> GenresIds,
        IReadOnlyCollection<Guid> CastMembersIds,
        IReadOnlyCollection<VideoModelOutputRelatedAggregate> Categories,
        IReadOnlyCollection<VideoModelOutputRelatedAggregate> Genres,
        IReadOnlyCollection<VideoModelOutputRelatedAggregate> CastMembers,
        string? ThumbFileUrl,
        string? BannerFileUrl,
        string? ThumbHalfFileUrl,
        string? VideoFileUrl,
        string? TrailerFileUrl
    )
    {
        public static VideoModelOutput FromVideo(DomainEntity.Video video)
            => new(
                video.Id,
                video.Title,
                video.Description,
                video.YearLaunched,
                video.Opened,
                video.Published,
                video.Duration,
                video.Rating.ToStringSignal(),
                video.CreatedAt,
                video.Categories,
                video.Genres,
                video.CastMembers,
                video.Categories.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
                video.Genres.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
                video.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
                video.Thumb?.Path,
                video.Banner?.Path,
                video.ThumbHalf?.Path,
                video.Media?.FilePath,
                video.Trailer?.FilePath
            );

        public static VideoModelOutput FromVideo(
            DomainEntity.Video video,
            IReadOnlyList<DomainEntity.Category>? categories,
            IReadOnlyList<DomainEntity.Genre>? genres
            IReadOnlyList<DomainEntity.CastMember>? castMembers
        )
            => new(
                video.Id,
                video.Title,
                video.Description,
                video.YearLaunched,
                video.Opened,
                video.Published,
                video.Duration,
                video.Rating.ToStringSignal(),
                video.CreatedAt,
                video.Categories,
                video.Genres,
                video.CastMembers,
                video.Categories.Select(id =>
                    new VideoModelOutputRelatedAggregate(id, categories?.FirstOrDefault(category => category.Id == id)?.Name)).ToList(),
                video.Genres.Select(id =>
                    new VideoModelOutputRelatedAggregate(id, genres?.FirstOrDefault(genre => genre.Id == id)?.Name)).ToList(),
                video.CastMembers.Select(id =>
                    new VideoModelOutputRelatedAggregate(id, castMembers?.FirstOrDefault(castMember => castMember.Id == id)?.Name)).ToList(),    
                video.Thumb?.Path,
                video.Banner?.Path,
                video.ThumbHalf?.Path,
                video.Media?.FilePath,
                video.Trailer?.FilePath
            );
    };

    public record VideoModelOutputRelatedAggregate(Guid Id, string? Name = null);
}
