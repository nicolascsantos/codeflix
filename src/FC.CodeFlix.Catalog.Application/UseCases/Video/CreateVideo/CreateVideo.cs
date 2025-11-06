
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.Validation;
using MediatR;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;


namespace FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo
{
    public class CreateVideo : ICreateVideo
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoRepository _videoRepository;

        public CreateVideo(IUnitOfWork unitOfWork, IVideoRepository videoRepository)
            => (_unitOfWork, _videoRepository) = (unitOfWork, videoRepository);
        

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

            if ((request.categoriesIds?.Count ?? 0) > 0)
                request.categoriesIds!.ToList().ForEach(video.AddCategory);
            

            var validationHandler = new NotificationValidationHandler();

            video.Validate(validationHandler);

            if (validationHandler.HasErrors())
                throw new EntityValidationException("There are validation errors", validationHandler.Errors);
            

            await _videoRepository.Insert(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return CreateVideoOutput.FromVideo(video);
        }
    }
}
