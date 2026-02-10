
using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using MediatR;

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

        public async Task<Unit> Handle(UploadMediasInput request, CancellationToken cancellationToken)
        {
            var video = await _videoRepository
                .Get(request.VideoId, cancellationToken);

            try
            {
                await UploadVideo(request, video, cancellationToken);
                await UploadTrailer(request, video, cancellationToken);

                await _videoRepository.Update(video, cancellationToken);
                await _unitOfWork.Commit(cancellationToken);
                return Unit.Value;
            }
            catch (Exception)
            {
                await ClearStorage(request, video, cancellationToken);
                throw;
            }
        }

        private async Task ClearStorage(UploadMediasInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (request.VideoFile is not null && video.Media is not null)
                await _storageService
                    .Delete(video.Media.FilePath, cancellationToken);

            if (request.TrailerFile is not null && video.Trailer is not null)
                await _storageService
                    .Delete(video.Trailer.FilePath, cancellationToken);
        }

        private async Task UploadTrailer(UploadMediasInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (request.TrailerFile is not null)
            {
                var mediaFileName = StorageName.Create(
                    video.Id,
                    nameof(video.Trailer),
                    request.TrailerFile.Extension
                );
                var uploadedFilePath = await _storageService.Upload(
                    mediaFileName,
                    request.TrailerFile.FileStream,
                    request.TrailerFile.ContentType,
                    cancellationToken
                );
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
                var uploadedFilePath = await _storageService.Upload(
                    mediaFileName,
                    request.VideoFile.FileStream,
                    request.VideoFile.ContentType,
                    cancellationToken
                );
                video.UpdateMedia(uploadedFilePath);
            }
        }
    }
}
