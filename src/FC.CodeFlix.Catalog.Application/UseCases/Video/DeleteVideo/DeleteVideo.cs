
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.DeleteVideo
{
    public class DeleteVideo : IDeleteVideo
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
            => (_videoRepository, _unitOfWork) = (videoRepository, unitOfWork);

        public async Task<Unit> Handle(DeleteVideoInput request, CancellationToken cancellationToken)
        {
            var video = await _videoRepository.Get(request.VideoId, cancellationToken);

            await _videoRepository.Delete(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return Unit.Value;
        }
    }
}
