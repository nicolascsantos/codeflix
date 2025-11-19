using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.GetVideo
{
    public class GetVideo : IGetVideo
    {
        private readonly IVideoRepository _videoRepository;

        public GetVideo(IVideoRepository videoRepository)
            => _videoRepository = videoRepository;


        public async Task<VideoModelOutput> Handle(GetVideoInput request, CancellationToken cancellationToken)
        {
            var video = await _videoRepository.Get(request.Id, cancellationToken);
            return VideoModelOutput.FromVideo(video);
        }

    }
}
