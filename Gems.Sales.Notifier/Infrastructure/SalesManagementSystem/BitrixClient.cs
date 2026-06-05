using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Dto;
using Gems.Sales.Notifier.Options;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem
{
    public class BitrixClient : ISalesManagementSystemClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<BitrixOptions> _bitrixOptions;
        public BitrixClient(HttpClient httpClient,IOptions<BitrixOptions> bitrixOptions)
        {
            _httpClient = httpClient;
            _bitrixOptions = bitrixOptions;
        }

        public async Task<string?> GetComment(long commentId, CancellationToken cancellationToken)
        {
            var payload = new { id = commentId };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{_bitrixOptions.Value.ApiUrl}/{_bitrixOptions.Value.ApiToken}/crm.timeline.comment.get";
            var response = await _httpClient.PostAsync($"{url}", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadFromJsonAsync<BitrixResponseWrapperDto>();

            return rawResponse?.Result.Comment;
        }
    }
}
