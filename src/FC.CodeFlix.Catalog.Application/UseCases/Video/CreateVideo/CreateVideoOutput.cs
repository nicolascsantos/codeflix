using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Enum;
using MediatR;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo
{
    public record CreateVideoOutput(
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
    ) : IRequest<VideoModelOutput>
    {
        public static CreateVideoOutput FromVideo(DomainEntity.Video video)
            => new(video.Id,
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
                   video.Trailer?.FilePath);
    }
}
