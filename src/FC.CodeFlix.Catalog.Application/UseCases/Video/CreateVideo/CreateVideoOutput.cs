using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Enum;
using MediatR;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo
{
    public record CreateVideoOutput(
        string Title,
        string Description,
        int YearLaunched,
        bool Opened,
        bool Published,
        int Duration,
        Rating Rating
    ) : IRequest<VideoModelOutput>
    {
        public static CreateVideoOutput FromVideo(DomainEntity.Video video)
            => new(video.Title,
                   video.Description,
                   video.YearLaunched,
                   video.Opened,
                   video.Published,
                   video.Duration,
                   video.Rating);
    }
}
