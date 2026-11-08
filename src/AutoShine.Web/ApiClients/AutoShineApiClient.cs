using System.Net.Http.Json;

namespace AutoShine.Web.ApiClients
{
    public class AutoShineApiClient : IAutoShineApiClient
    {
        private readonly HttpClient _httpClient;
        
        public AutoShineApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HealthResponse?> GetHealthAsync(CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<HealthResponse>("api/v1/health", cancellationToken);
        }
    }
}
