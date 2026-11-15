
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

            await DeleteMedias(trailerFilePath, mediaFilePath, cancellationToken);
            await _videoRepository.Delete(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return Unit.Value;
        }

        private async Task DeleteMedias(string trailerFilePath, string mediaFilePath, CancellationToken cancellationToken)
        {
            if (trailerFilePath is not null)
                await _storageService.Delete(trailerFilePath, cancellationToken);

            if (mediaFilePath is not null)
                await _storageService.Delete(mediaFilePath, cancellationToken);
        }
    }
}
