using FC.CodeFlix.Catalog.Application.Common;

namespace FC.CodeFlix.Catalog.API.APIModels.Response
{
    public class APIResponseList<TItemData> : APIResponse<IReadOnlyList<TItemData>>
    {
        public APIResponseListMeta Meta { get; private set; }

        public APIResponseList(int currentPage, int perPage, int total, IReadOnlyList<TItemData> data) : base(data)
            => Meta = new(currentPage, perPage, total);
        

        public APIResponseList(PaginatedListOutput<TItemData> paginatedListOutput) : base(paginatedListOutput.Items)
            => Meta = new(paginatedListOutput.Page, paginatedListOutput.PerPage, paginatedListOutput.Total);
    }
}
