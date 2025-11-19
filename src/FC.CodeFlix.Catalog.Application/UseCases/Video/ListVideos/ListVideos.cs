
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos
{
    public class ListVideos : IListVideos
    {
        private readonly IVideoRepository _videoRepository;

        public ListVideos(IVideoRepository videoRepository)
            => _videoRepository = videoRepository;


        public async Task<ListVideosOutput> Handle(ListVideosInput request, CancellationToken cancellationToken)
        {

            var searchResult = await _videoRepository
                .Search(request.ToSearchInput(), cancellationToken);

            return new ListVideosOutput(
                searchResult.CurrentPage,
                searchResult.PerPage,
                searchResult.Total,
                searchResult.Items.Select(VideoModelOutput.FromVideo).ToList()
            );
        }
    }
}
