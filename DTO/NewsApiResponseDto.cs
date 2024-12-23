using System.Text.Json.Serialization;

namespace News_Aggregator.DTO
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);


    public class NewsApiResponseDto
    {

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("totalResults")]
        public int totalResults { get; set; }

        [JsonPropertyName("articles")]
        public List<Article> articles { get; set; }
    }
    public class Article
    {
        [JsonPropertyName("source")]
        public Source Source { get; set; }

        [JsonPropertyName("author")]
        public string author { get; set; }

        [JsonPropertyName("title")]
        public string title { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("url")]
        public string url { get; set; }

        [JsonPropertyName("urlToImage")]
        public string urlToImage { get; set; }

        [JsonPropertyName("publishedAt")]
        public DateTime publishedAt { get; set; }

        [JsonPropertyName("content")]
        public string content { get; set; }
    }



    public class Source
    {
        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }
    }
}

