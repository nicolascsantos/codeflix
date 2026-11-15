
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
                await UploadImages(request, video, cancellationToken);

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

            if (request.BannerFile is not null && video.Banner is not null)
                await _storageService
                    .Delete(video.Banner.Path, cancellationToken);

            if (request.ThumbFile is not null && video.Thumb is not null)
                await _storageService
                    .Delete(video.Thumb.Path, cancellationToken);

            if (request.ThumbHalfFile is not null && video.ThumbHalf is not null)
                await _storageService
                    .Delete(video.ThumbHalf.Path, cancellationToken);
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

        private async Task UploadImages(UploadMediasInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (request.BannerFile is not null)
            {
                var mediaName = StorageName.Create(
                    video.Id,
                    nameof(video.Banner),
                    request.BannerFile.Extension
                );

                var uploadedFilePath = await _storageService.Upload(
                    mediaName,
                    request.BannerFile.FileStream,
                    request.BannerFile.ContentType,
                    cancellationToken
                );

                video.UpdateBanner(uploadedFilePath);
            }

            if (request.ThumbFile is not null)
            {
                var mediaName = StorageName.Create(
                    video.Id,
                    nameof(video.Thumb),
                    request.ThumbFile.Extension
                );

                var uploadedFilePath = await _storageService.Upload(
                    mediaName,
                    request.ThumbFile.FileStream,
                    request.ThumbFile.ContentType,
                    cancellationToken
                );

                video.UpdateThumb(uploadedFilePath);
            }

            if (request.ThumbHalfFile is not null)
            {
                var mediaName = StorageName.Create(
                    video.Id,
                    nameof(video.Thumb),
                    request.ThumbHalfFile.Extension
                );

                var uploadedFilePath = await _storageService.Upload(
                    mediaName,
                    request.ThumbHalfFile.FileStream,
                    request.ThumbHalfFile.ContentType,
                    cancellationToken
                );

                video.UpdateThumbHalf(uploadedFilePath);
            }
        }
    }
}
