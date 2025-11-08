
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.Validation;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;


namespace FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo
{
    public class CreateVideo : ICreateVideo
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICastMemberRepository _castMemberRepository;
        private readonly IStorageService _storageService;

        public CreateVideo(
            IUnitOfWork unitOfWork,
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository,
            IGenreRepository genreRepository,
            ICastMemberRepository castMemberRepository,
            IStorageService storageService
        )
            => (
                _unitOfWork,
                _videoRepository,
                _categoryRepository,
                _genreRepository,
                _castMemberRepository,
                _storageService
            ) = (unitOfWork, videoRepository, categoryRepository, genreRepository, castMemberRepository, storageService);


        public async Task<CreateVideoOutput> Handle(CreateVideoInput request, CancellationToken cancellationToken)
        {
            var video = new DomainEntity.Video(
                request.Title,
                request.Description,
                request.YearLaunched,
                request.Opened,
                request.Published,
                request.Duration,
                request.Rating
            );

            if ((request.CategoriesIds?.Count ?? 0) > 0)
            {
                request.CategoriesIds!.ToList().ForEach(video.AddCategory);
                await ValidateCategoriesIds(request, cancellationToken);
            }

            if ((request.GenresIds?.ToList().Count ?? 0) > 0)
            {
                request.GenresIds!.ToList().ForEach(video.AddGenre);
                await ValidateGenresIds(request, cancellationToken);
            }

            if ((request.CastMembersIds?.ToList().Count ?? 0) > 0)
            {
                request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
                await ValidateCastMembersIds(request, cancellationToken);
            }

            if (request.Thumb is not null)
            {
                var thumbUrl = await _storageService
                    .Upload($"{video.Id}-thumb.{request.Thumb.Extension}", request.Thumb.FileStream, cancellationToken);
                video.UpdateThumb(thumbUrl);
            }

            if (request.Banner is not null)
            {
                var bannerUrl = await _storageService
                    .Upload($"{video.Id}-banner.{request.Banner.Extension}", request.Banner.FileStream, cancellationToken);
                video.UpdateBanner(bannerUrl);
            }


            var validationHandler = new NotificationValidationHandler();

            video.Validate(validationHandler);

            if (validationHandler.HasErrors())
                throw new EntityValidationException("There are validation errors", validationHandler.Errors);


            await _videoRepository.Insert(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return CreateVideoOutput.FromVideo(video);
        }

        private async Task ValidateCategoriesIds(CreateVideoInput request, CancellationToken cancellationToken)
        {
            var idsInPersistence = await _categoryRepository.GetIdsListByIds(request.CategoriesIds!.ToList(), cancellationToken);
            if (idsInPersistence.Count < request.CategoriesIds!.Count)
            {
                var notFoundIds = request.CategoriesIds.ToList().FindAll(x => !idsInPersistence.Contains(x));
                var notFoundIdsAsString = string.Join(";", notFoundIds);
                throw new RelatedAggregateException($"Related category id or ids not found: '{notFoundIdsAsString}'");
            }
        }

        private async Task ValidateGenresIds(CreateVideoInput request, CancellationToken cancellationToken)
        {
            var idsInPersistence = await _genreRepository.GetIdsListByIds(request.GenresIds!.ToList(), cancellationToken);
            if (idsInPersistence.Count < request.GenresIds!.Count)
            {
                var notFoundIds = request.GenresIds.ToList().FindAll(x => !idsInPersistence.Contains(x));
                var notFoundIdsAsString = string.Join(";", notFoundIds);
                throw new RelatedAggregateException($"Related genre id or ids not found: '{notFoundIdsAsString}'");
            }
        }

        private async Task ValidateCastMembersIds(CreateVideoInput request, CancellationToken cancellationToken)
        {
            var idsInPersistence = await _castMemberRepository.GetIdsListByIds(request.CastMembersIds!.ToList(), cancellationToken);
            if (idsInPersistence.Count < request.CastMembersIds!.Count)
            {
                var notFoundIds = request.CastMembersIds.ToList().FindAll(x => !idsInPersistence.Contains(x));
                var notFoundIdsAsString = string.Join(";", notFoundIds);
                throw new RelatedAggregateException($"Related genre id or ids not found: '{notFoundIdsAsString}'");
            }
        }
    }
}
