using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos
{
    public class ListVideosOutput : PaginatedListOutput<VideoModelOutput>
    {
        public ListVideosOutput(
            int page,
            int perPage,
            int total,
            IReadOnlyList<VideoModelOutput> items
            ) : base(page, perPage, total, items)
        {
        }

        public ListVideosOutput FromSearchOutput(SearchOutput<DomainEntity.Video> searchOutput)
            => new(
                searchOutput.CurrentPage,
                searchOutput.PerPage,
                searchOutput.Total,
                searchOutput.Items.Select(video => VideoModelOutput.FromVideo(video)).ToList().AsReadOnly()
            );
    }
}
