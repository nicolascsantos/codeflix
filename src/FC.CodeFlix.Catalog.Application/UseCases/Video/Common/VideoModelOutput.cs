using FC.CodeFlix.Catalog.Application.UseCases.Video.GetVideo;
using FC.CodeFlix.Catalog.Domain.Enum;
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
        Rating Rating,
        DateTime CreatedAt,
        IReadOnlyCollection<Guid> Categories,
        IReadOnlyCollection<Guid> Genres,
        IReadOnlyCollection<Guid> CastMembers,
        string? Thumb,
        string? Banner,
        string? ThumbHalf,
        string? Media,
        string? Trailer
    )
    {
        public static GetVideoOutput FromVideo(DomainEntity.Video video)
            => new(
                video.Id,
                video.Title,
                video.Description,
                video.YearLaunched,
                video.Opened,
                video.Published,
                video.Duration,
                video.Rating,
                video.CreatedAt,
                video.Categories,
                video.Genres,
                video.CastMembers,
                video.Thumb?.Path,
                video.Banner?.Path,
                video.ThumbHalf?.Path,
                video.Media?.FilePath,
                video.Trailer?.FilePath
            );
    };
}
