using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Dto;
using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Models;
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
        public BitrixClient(HttpClient httpClient, IOptions<BitrixOptions> bitrixOptions)
        {
            _httpClient = httpClient;
            _bitrixOptions = bitrixOptions;
        }
        public async Task<Comment?> GetComment(long commentId, CancellationToken cancellationToken)
        {
            var payload = new { id = commentId };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{_bitrixOptions.Value.ApiUrl}/{_bitrixOptions.Value.ApiToken}/crm.timeline.comment.get";
            var response = await _httpClient.PostAsync($"{url}", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadFromJsonAsync<BitrixResponseWrapperDto>();

            if (rawResponse == null)
            {
                return null;
            }

            var commentModel = new Comment
            {
                Id = Convert.ToInt32(rawResponse.Result.Id),
                DealId = Convert.ToInt32(rawResponse.Result.EntityId),
                Body = rawResponse.Result.Comment
            };

            return commentModel;
        }
        public async Task<Deal?> GetDeal(long dealId, CancellationToken cancellationToken)
        {
            var payload = new { id = dealId, entityTypeId = 2 };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{_bitrixOptions.Value.ApiUrl}/{_bitrixOptions.Value.ApiToken}/crm.item.get";
            var response = await _httpClient.PostAsync($"{url}", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadFromJsonAsync<BitrixDealResponseWrapperDto>();

            if (rawResponse == null)
            {
                return null;
            }

            var dealModel = new Deal
            {
                Id = rawResponse.Result.Item.Id,
                Title = rawResponse.Result.Item.Title
            };

            return dealModel;
        }
    }
}
