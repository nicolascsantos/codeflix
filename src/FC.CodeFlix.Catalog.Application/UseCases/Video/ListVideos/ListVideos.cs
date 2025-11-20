
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Repository;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos
{
    public class ListVideos : IListVideos
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ListVideos(
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository
        )
        {
            _videoRepository = videoRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ListVideosOutput> Handle(ListVideosInput request, CancellationToken cancellationToken)
        {

            var searchResult = await _videoRepository
                .Search(request.ToSearchInput(), cancellationToken);
            IReadOnlyList<DomainEntities.Category>? categories = null;
            var relatedCategoriesIds = searchResult.Items
                .SelectMany(video => video.Categories)
                .Distinct()
                .ToList();

            if (relatedCategoriesIds is not null && relatedCategoriesIds.Count > 0)
            {
                categories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
            }

            var output =  new ListVideosOutput(
                searchResult.CurrentPage,
                searchResult.PerPage,
                searchResult.Total,
                searchResult.Items.Select(item => VideoModelOutput.FromVideo(item, categories)).ToList()
            );

            return output;
        }
    }
}
