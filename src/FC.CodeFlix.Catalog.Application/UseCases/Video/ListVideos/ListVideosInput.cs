using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos
{
    public class ListVideosInput : PaginatedListInput, IRequest<ListVideosOutput>
    {
        public ListVideosInput(
            int page,
            int perPage,
            string search, 
            string sort,
            SearchOrder dir) 
            : base(page, perPage, search, sort, dir)
        {
        }

        public ListVideosInput() : base(1, 15, "", "", SearchOrder.Asc)
        {
            
        }
    }
}
