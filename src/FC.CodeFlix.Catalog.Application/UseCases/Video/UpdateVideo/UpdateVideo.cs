using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;
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
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVideo(
            IVideoRepository videoRepository,
            IGenreRepository genreRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork
        )
        {
            _videoRepository = videoRepository;
            _genreRepository = genreRepository;
            _categoryRepository = categoryRepository;
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

            await ValidateAndAddRelationships(request, video, cancellationToken);

            var validationHandler = new NotificationValidationHandler();
            video.Validate(validationHandler);
            if (validationHandler.HasErrors())
                throw new EntityValidationException("There are validation errors.", validationHandler.Errors);

            await _videoRepository.Update(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return VideoModelOutput.FromVideo(video);
        }

        private async Task ValidateAndAddRelationships(UpdateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
        {
            if ((request.GenresIds?.ToList().Count ?? 0) > 0)
            {
                await ValidateGenresIds(request, cancellationToken);
                video.RemoveAllGenres();
                request.GenresIds!.ToList().ForEach(video.AddGenre);
            }

            if ((request.CategoriesIds?.ToList().Count ?? 0) > 0)
            {
                await ValidateCategoriesIds(request, cancellationToken);
                video.RemoveAllCategories();
                request.CategoriesIds!.ToList().ForEach(video.AddCategory);
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
    }
}
