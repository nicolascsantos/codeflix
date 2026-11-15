
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.DeleteVideo
{
    public class DeleteVideo : IDeleteVideo
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public DeleteVideo(
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            IStorageService storageService
        )
            => (_videoRepository, _unitOfWork, _storageService) = (videoRepository, unitOfWork, storageService);

        public async Task<Unit> Handle(DeleteVideoInput request, CancellationToken cancellationToken)
        {
            var video = await _videoRepository.Get(request.VideoId, cancellationToken);
            var trailerFilePath = video.Trailer?.FilePath;
            var mediaFilePath = video.Media?.FilePath;
            var bannerFilePath = video.Banner?.Path;
            var thumbFilePath = video.Thumb?.Path;
            var thumbHalfFilePath = video.ThumbHalf?.Path;

            await DeleteMedias(trailerFilePath, mediaFilePath, cancellationToken);
            await ClearImageMedia(bannerFilePath, thumbFilePath, thumbHalfFilePath, cancellationToken);
            await _videoRepository.Delete(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return Unit.Value;
        }

        private async Task DeleteMedias(string? trailerFilePath, string? mediaFilePath, CancellationToken cancellationToken)
        {
            if (trailerFilePath is not null)
                await _storageService.Delete(trailerFilePath, cancellationToken);

            if (mediaFilePath is not null)
                await _storageService.Delete(mediaFilePath, cancellationToken);
        }

        private async Task ClearImageMedia(string? bannerFilePath, string? thumbFilePath, string? thumbHalfFilePath, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(bannerFilePath))
                await _storageService.Delete(bannerFilePath, cancellationToken);

            if (!string.IsNullOrEmpty(thumbFilePath))
                await _storageService.Delete(thumbFilePath, cancellationToken);

            if (!string.IsNullOrEmpty(thumbHalfFilePath))
                await _storageService.Delete(thumbHalfFilePath, cancellationToken);
        }
    }
}
