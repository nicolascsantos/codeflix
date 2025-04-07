namespace FC.CodeFlix.Catalog.API.APIModels.Response
{
    public class APIResponse<TData> where TData : class
    {
        public TData Data { get; set; }

        public APIResponse(TData data)
            => Data = data;
    }
}
