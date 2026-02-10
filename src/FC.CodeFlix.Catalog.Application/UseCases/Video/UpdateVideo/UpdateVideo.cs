using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.Validation;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo
{
    public class UpdateVideo : IUpdateVideo
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICastMemberRepository _castMemberRepository;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVideo(
            IVideoRepository videoRepository,
            IGenreRepository genreRepository,
            ICategoryRepository categoryRepository,
            ICastMemberRepository castMemberRepository,
            IStorageService storageService,
            IUnitOfWork unitOfWork
        )
        {
            _videoRepository = videoRepository;
            _genreRepository = genreRepository;
            _categoryRepository = categoryRepository;
            _castMemberRepository = castMemberRepository;
            _storageService = storageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<VideoModelOutput> Handle(UpdateVideoInput request, CancellationToken cancellationToken)
        {
            var video = await _videoRepository.Get(request.VideoId, cancellationToken);

            video.Update(
                request.Title,
                request.Description,
                request.YearLaunched,
                request.Opened,
                request.Published,
                request.Duration,
                request.Rating
            );

            var validationHandler = new NotificationValidationHandler();
            video.Validate(validationHandler);
            if (validationHandler.HasErrors())
                throw new EntityValidationException("There are validation errors.", validationHandler.Errors);

            await ValidateAndAddRelationships(request, video, cancellationToken);
            await UploadMediaImages(request, video, cancellationToken);

            await _videoRepository.Update(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return VideoModelOutput.FromVideo(video);
        }

        private async Task ValidateAndAddRelationships(UpdateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
        {
            if (request.GenresIds is not null)
            {
                video.RemoveAllGenres();
                if (request.GenresIds.Count > 0)
                    await ValidateGenresIds(request, cancellationToken);
                request.GenresIds!.ToList().ForEach(video.AddGenre);
            }

            if (request.CategoriesIds is not null)
            {
                video.RemoveAllCategories();
                if (request.CategoriesIds.Count > 0)
                {
                    await ValidateCategoriesIds(request, cancellationToken);
                    request.CategoriesIds!.ToList().ForEach(video.AddCategory);
                }

            }

            if (request.CastMembersIds is not null)
            {
                video.RemoveAllCastMembers();
                if (request.CastMembersIds.Count > 0)
                {
                    await ValidateCastMembersIds(request, cancellationToken);
                    request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
                }
            }

            if ((request.CastMembersIds?.ToList().Count ?? 0) > 0)
            {
                await ValidateCastMembersIds(request, cancellationToken);
                video.RemoveAllCastMembers();
                request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
            }
        }

        private async Task ValidateGenresIds(UpdateVideoInput request, CancellationToken cancellationToken)
        {
            var idsInPersistence = await _genreRepository.GetIdsListByIds(request.GenresIds!.ToList(), cancellationToken);
            if (idsInPersistence.Count < request.GenresIds!.Count)
            {
                var notFoundIds = request.GenresIds.ToList().FindAll(x => !idsInPersistence.Contains(x));
                var notFoundIdsAsString = string.Join(";", notFoundIds);
                throw new RelatedAggregateException($"Related genre id or ids not found: '{notFoundIdsAsString}'");
            }
        }

        private async Task ValidateCategoriesIds(UpdateVideoInput request, CancellationToken cancellationToken)
        {
            var idsInPersistence = await _categoryRepository.GetIdsListByIds(request.CategoriesIds!.ToList(), cancellationToken);
            if (idsInPersistence.Count < request.CategoriesIds!.Count)
            {
                var notFoundIds = request.CategoriesIds.ToList().FindAll(x => !idsInPersistence.Contains(x));
                var notFoundIdsAsString = string.Join(";", notFoundIds);
                throw new RelatedAggregateException($"Related category id or ids not found: '{notFoundIdsAsString}'");
            }
        }

        private async Task ValidateCastMembersIds(UpdateVideoInput request, CancellationToken cancellationToken)
        {
            var idsInPersistence = await _castMemberRepository.GetIdsListByIds(request.CastMembersIds!.ToList(), cancellationToken);
            if (idsInPersistence.Count < request.CastMembersIds!.Count)
            {
                var notFoundIds = request.CastMembersIds.ToList().FindAll(x => !idsInPersistence.Contains(x));
                var notFoundIdsAsString = string.Join(";", notFoundIds);
                throw new RelatedAggregateException($"Related cast member id or ids not found: '{notFoundIdsAsString}'");
            }
        }

        private async Task UploadMediaImages(
            UpdateVideoInput request,
            DomainEntity.Video video,
            CancellationToken cancellationToken
        )
        {
            if (request.Banner is not null)
            {
                var fileName = StorageName.Create(request.VideoId, nameof(request.Banner), request.Banner.Extension);
                var bannerUrl = await _storageService.Upload(
                    fileName,
                    request.Banner.FileStream,
                    cancellationToken
                );
                video.UpdateBanner(bannerUrl);
            }

            if (request.Thumb is not null)
            {
                var fileName = StorageName.Create(request.VideoId, nameof(request.Thumb), request.Thumb.Extension);
                var thumbUrl = await _storageService.Upload(
                    fileName,
                    request.Thumb.FileStream,
                    cancellationToken
                );
                video.UpdateThumb(thumbUrl);
            }

            if (request.ThumbHalf is not null)
            {
                var fileName = StorageName.Create(request.VideoId, nameof(request.ThumbHalf), request.ThumbHalf.Extension);
                var thumbUrl = await _storageService.Upload(
                    fileName,
                    request.ThumbHalf.FileStream,
                    cancellationToken
                );
                video.UpdateThumbHalf(thumbUrl);
            }
        }
    }
}
