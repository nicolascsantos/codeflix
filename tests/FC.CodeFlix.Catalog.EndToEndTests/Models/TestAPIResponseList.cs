namespace FC.CodeFlix.Catalog.EndToEndTests.Models
{
    public class TestAPIResponseList<TOutputItem> : TestAPIResponse<IReadOnlyList<TOutputItem>>
    {
        public TestAPIResponseList(IReadOnlyList<TOutputItem> data, TestAPIResponseListMeta meta) : base(data)
        {
            Meta = meta;
        }

        public TestAPIResponseListMeta Meta { get; set; }
    }

    public class TestAPIResponseListMeta
    {
        public int CurrentPage { get; set; }

        public int PerPage { get; set; }

        public int Total { get; set; }

        public TestAPIResponseListMeta(int currentPage, int perPage, int total)
        {
            CurrentPage = currentPage;
            PerPage = perPage;
            Total = total;
        }
    }

    public class TestAPIResponse<TData> where TData : class
    {
        public TData Data { get; set; }

        public TestAPIResponse(TData data)
            => Data = data;
    }
}
