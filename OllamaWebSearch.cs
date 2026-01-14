// OllamaWebSearch.cs - C# SDK for Ollama Web Search and Fetch APIs
// Version: 1.0.0
// Usage:
// var client = new OllamaWebSearchClient("your_api_key");
// var results = await client.WebSearchAsync("what is ollama?");

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OllamaWebSearch
{
    /// <summary>
    /// Web search result item
    /// </summary>
    public class WebSearchResult
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    /// <summary>
    /// Web search response
    /// </summary>
    public class WebSearchResponse
    {
        [JsonPropertyName("results")]
        public List<WebSearchResult> Results { get; set; }
    }

    /// <summary>
    /// Web fetch response
    /// </summary>
    public class WebFetchResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("links")]
        public List<string> Links { get; set; }
    }

    /// <summary>
    /// Web search request
    /// </summary>
    public class WebSearchRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("max_results")]
        public int? MaxResults { get; set; }
    }

    /// <summary>
    /// Web fetch request
    /// </summary>
    public class WebFetchRequest
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// Ollama API exception
    /// </summary>
    public class OllamaApiException : Exception
    {
        public int StatusCode { get; }

        public OllamaApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }


    /// <summary>
    /// Ollama Web Search client
    /// </summary>
    public class OllamaWebSearchClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://ollama.com/api";
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initialize Ollama Web Search client
        /// </summary>
        /// <param name="apiKey">API key from https://ollama.com/settings/keys</param>
        /// <param name="baseUrl">Base API URL, default: https://ollama.com/api</param>
        /// <param name="timeoutSeconds">Request timeout in seconds, default: 30</param>
        public OllamaWebSearchClient(string apiKey, string baseUrl = null, int timeoutSeconds = 30)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API key cannot be empty");

            _apiKey = apiKey;

            if (!string.IsNullOrEmpty(baseUrl))
                _baseUrl = baseUrl.TrimEnd('/');

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", apiKey),
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Execute web search
        /// </summary>
        /// <param name="query">Search query string</param>
        /// <param name="maxResults">Maximum number of results (default: 5, max: 10)</param>
        /// <returns>List of search results</returns>
        public async Task<List<WebSearchResult>> WebSearchAsync(string query, int? maxResults = 5)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("Search query cannot be empty");

            var request = new WebSearchRequest
            {
                Query = query,
                MaxResults = maxResults
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/web_search", content);
            await EnsureSuccess(response);

            var responseJson = await response.Content.ReadAsStringAsync();
            var searchResponse = JsonSerializer.Deserialize<WebSearchResponse>(responseJson, _jsonOptions);

            return searchResponse?.Results ?? new List<WebSearchResult>();
        }

        /// <summary>
        /// Fetch single webpage content
        /// </summary>
        /// <param name="url">URL to fetch</param>
        /// <returns>Webpage content information</returns>
        public async Task<WebFetchResponse> WebFetchAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be empty");
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            var request = new WebFetchRequest { Url = url };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/web_fetch", content);
            await EnsureSuccess(response);

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WebFetchResponse>(responseJson, _jsonOptions);
        }

        /// <summary>
        /// Batch search multiple queries
        /// </summary>
        public async Task<Dictionary<string, List<WebSearchResult>>> BatchSearchAsync(IEnumerable<string> queries)
        {
            var results = new Dictionary<string, List<WebSearchResult>>();

            foreach (var query in queries)
            {
                try
                {
                    var searchResults = await WebSearchAsync(query);
                    results[query] = searchResults;
                }
                catch (Exception)
                {
                    results[query] = new List<WebSearchResult>();
                }
            }

            return results;
        }

        /// <summary>
        /// Validate if API key is valid
        /// </summary>
        public async Task<bool> ValidateApiKeyAsync()
        {
            try
            {
                // Try a simple search to validate key
                var results = await WebSearchAsync("test", maxResults: 1);
                return results != null;
            }
            catch (OllamaApiException ex) when (ex.StatusCode == 401)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new OllamaApiException(
                    $"API request failed (status: {(int)response.StatusCode}): {errorContent}",
                    (int)response.StatusCode);
            }
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OllamaWebSearchClient()
        {
            Dispose(false);
        }

    }

}