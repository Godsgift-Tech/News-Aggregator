using News_Aggregator.EntityModel;

namespace News_Aggregator.Models.DomainModel
{
    public class SavedArticle
    {
        public Guid SavedArticleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public Guid UserId { get; set; }
        public DateTime SavedDate { get; set; }
        public string Url { get; set; }
        public ApplicationUser User { get; set; }
    }
}
