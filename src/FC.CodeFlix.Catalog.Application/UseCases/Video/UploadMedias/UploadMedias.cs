
using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias
{
    public class UploadMedias : IUploadMedias
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public UploadMedias(IVideoRepository videoRepository, IUnitOfWork unitOfWork, IStorageService storageService)
        {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task Handle(UploadMediasInput request, CancellationToken cancellationToken)
        {
            var video = await _videoRepository
                .Get(request.VideoId, cancellationToken);

            await UploadVideo(request, video, cancellationToken);
            await UploadTrailer(request, video, cancellationToken);

            await _videoRepository.Update(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
        }

        private async Task UploadTrailer(UploadMediasInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (request.TrailerFile is not null)
            {
                var mediaFileName = StorageName.Create(
                    video.Id,
                    nameof(video.Media),
                    request.TrailerFile.Extension
                );
                var uploadedFilePath = await _storageService
                    .Upload(mediaFileName, request.TrailerFile.FileStream, cancellationToken);
                video.UpdateTrailer(uploadedFilePath);
            }
        }

        private async Task UploadVideo(UploadMediasInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (request.VideoFile is not null)
            {
                var mediaFileName = StorageName.Create(
                    video.Id,
                    nameof(video.Media),
                    request.VideoFile.Extension
                );
                var uploadedFilePath = await _storageService
                    .Upload(mediaFileName, request.VideoFile.FileStream, cancellationToken);
                video.UpdateMedia(uploadedFilePath);
            }
        }
    }
}
