using News_Aggregator.DTO;

namespace News_Aggregator.Service
{
    public class ArticleService : IArticleService
    {
        private readonly NewsService _newsService;

        public ArticleService(NewsService newsService)
        {
            _newsService = newsService;
        }

        // Ensure the return type is Task<Article?>
        public async Task<Article> GetArticleDetailsByUrlAsync(string url)
        {
            // Fetch articles from the API
            var newsResponse = await _newsService.GetTopHeadlinesAsync();

            if (newsResponse == null || newsResponse.articles == null || !newsResponse.articles.Any())
            {
                return null;
            }

            // Find the article by URL
            var articleDto = newsResponse.articles.FirstOrDefault(a => a.url == url);

            if (articleDto == null)
            {
                return null; // Return null if the article is not found
            }

            // Map DTO to domain model
            var article = new Article
            {
                title = articleDto.title,
                description = articleDto.description,
                url = articleDto.url,
                Source = articleDto.Source,
                content = articleDto.content,
                publishedAt = (DateTime)articleDto.publishedAt
            };

            return article; // Return the article details
        }
    }
}

