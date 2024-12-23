using News_Aggregator.Models.DomainModel;

namespace News_Aggregator.Controllers
{
    internal class PaginationModel<T>
    {
        public List<SavedArticle> Items { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}