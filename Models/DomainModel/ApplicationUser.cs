using Microsoft.AspNetCore.Identity;
using News_Aggregator.Models.DomainModel;

namespace News_Aggregator.EntityModel
{
    public class ApplicationUser : IdentityUser<Guid>
    {
  

        public virtual ICollection<SavedHistory>? SearchHistories { get; set; } // One-to-many relationship with SearchHistory

        // Gets or sets the collection of saved articles associated with the user.
        public virtual ICollection<SavedArticle>? SavedArticles { get; set; } // One-to-many relationship with SavedArticle
    }
}
