
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

        public CreateVideo(IUnitOfWork unitOfWork, IVideoRepository videoRepository, ICategoryRepository categoryRepository, IGenreRepository genreRepository)
            => (
            _unitOfWork,
            _videoRepository,
            _categoryRepository,
            _genreRepository) = (unitOfWork, videoRepository, categoryRepository, genreRepository);


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
    }
}
