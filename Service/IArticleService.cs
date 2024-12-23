using News_Aggregator.DTO;
using News_Aggregator.EntityModel;
using News_Aggregator.Models.DomainModel;

namespace News_Aggregator.Service
{
    public interface IArticleService
    {
       // Task<SavedArticleEntity> GetArticleByIdAsync(Guid id); // Fetch article by ID
        Task <Article>GetArticleDetailsByUrlAsync(string url);
       // Task<List<SavedArticle>> GetArticlesByUserIdAsync(Guid userId); // Fetch articles by user ID
    }
}
