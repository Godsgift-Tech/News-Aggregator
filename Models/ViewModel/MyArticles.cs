using News_Aggregator.EntityModel;

namespace News_Aggregator.Models.ViewModel
{
    public class MyArticles
    {
        public Guid Id { get; set; } // unique identifier
        public string Title { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public DateTime PublishedAt { get; set; }

        // Navigation property to the user who created the article
        public Guid? UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Other properties and relationships as needed

    }
}
