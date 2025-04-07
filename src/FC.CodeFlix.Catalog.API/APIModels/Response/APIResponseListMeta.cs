namespace FC.CodeFlix.Catalog.API.APIModels.Response
{
    public class APIResponseListMeta
    {

        public int CurrentPage { get; set; }

        public int PerPage { get; set; }

        public int Total { get; set; }  

        public APIResponseListMeta(int currentPage, int perPage, int total)
        {
            CurrentPage = currentPage;
            PerPage = perPage;
            Total = total;
        }
    }
}
