using Newtonsoft.Json;
using News_Aggregator.DTO;

namespace News_Aggregator.Service
{
    public class NewsService
    {
        private readonly HttpClient _httpClient;

        // Constructor now only accepts the HttpClient
        public NewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<NewsApiResponseDto> GetTopHeadlinesAsync(string category = "", string query = "")
        {
            var url = "top-headlines?country=us"; // Base URL without the API key

            // Add category if provided
            if (!string.IsNullOrEmpty(category))
            {
                url += $"&category={category}";
            }

            // Add search query if provided
            if (!string.IsNullOrEmpty(query))
            {
                url += $"&q={query}";
            }

            // Make the HTTP request
            var response = await _httpClient.GetAsync(url);

            // Handle failed requests and log response content
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}. Response: {content}");
            }

            // Deserialize the JSON response
            var jsonString = await response.Content.ReadAsStringAsync();
            var newsApiResponse = JsonConvert.DeserializeObject<NewsApiResponseDto>(jsonString);

            return newsApiResponse;
        }
    }
}


