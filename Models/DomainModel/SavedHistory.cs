using News_Aggregator.EntityModel;

namespace News_Aggregator.Models.DomainModel
{
    public class SavedHistory
    {
        public Guid SearchHistoryId { get; set; }
        public Guid UserId { get; set; } //key corresponding to a unique User
        public string? SearchQuery { get; set; }
        public DateTime SearchDate { get; set; }
        public ApplicationUser? User { get; set; }//Many to one relationship
    }
}
