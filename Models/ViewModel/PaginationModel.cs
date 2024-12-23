using News_Aggregator.Models.DomainModel;

namespace News_Aggregator.Models.ViewModel
{
    public class PaginationModel
    {
        public List<SavedArticle> Items { get; set; } // The items on the current page
        public int TotalCount { get; set; } // Total number of items in the database
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
