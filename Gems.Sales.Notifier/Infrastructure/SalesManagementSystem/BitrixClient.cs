using Gems.Sales.Notifier.Application;
using Gems.Sales.Notifier.Options;
using Gems.Sales.Notifier.UseCases.NotifyTaggedUsers;
using MAX.Bot.Interfaces.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Dto.BitrixWebhookRequestDto;

namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem
{
    public class BitrixClient : ISalesManagementSystemClient
    {
        private readonly HttpClient _httpClient;
        private readonly IRequestQueue<long> _queue;
        private readonly IOptions<BitrixOptions> _bitrixOptions;
        private readonly ISender _sender;
        public BitrixClient(
            HttpClient httpClient,
            IRequestQueue<long> queue,
            IOptions<BitrixOptions> bitrixOptions,
            ISender sender)
        {
            _httpClient = httpClient;
            _queue = queue;
            _bitrixOptions = bitrixOptions;
            _sender = sender;
        }

        public async Task GetComment()
        {
            long? commentId = _queue.Dequeue();

            if (commentId is null)
            {
                return;
            }
            // Формируем тело запроса: {"id": 26980}
            var payload = new { id = commentId.Value };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{_bitrixOptions.Value.ApiUrl}/{_bitrixOptions.Value.ApiToken}/crm.timeline.comment.get";
            Log.Information($"commentId:{commentId}\nUrl:{url}");
            var response = await _httpClient.PostAsync($"{url}", content);
            response.EnsureSuccessStatusCode();

            // ВСЕГДА читаем сырой ответ
            string rawResponse = await response.Content.ReadAsStringAsync();
            Log.Information($"Status: {response.StatusCode}");

            response.EnsureSuccessStatusCode();

            // Пробуем десериализовать
            var wrapper = JsonSerializer.Deserialize<BitrixResponseWrapper>(rawResponse);

            if (wrapper?.Result != null)
            {
                Log.Information($"ID: {wrapper.Result.Id}");
                Log.Information($"COMMENT: {(string.IsNullOrEmpty(wrapper.Result.Comment) ? "false" : "true")}");
                long[] userIds = ExtractUserId(wrapper.Result.Comment);
                Log.Information($"BitrixUserIds: {string.Join(", ", userIds)}");
                /*
                var command = new NotifyTaggedUsersCommand(userIds);
                CancellationToken ct = new(); //ПЕРЕДЕЛАТЬ
                await _sender.Send(command, ct);
                */

            }
            else
            {
                Log.Information("wrapper или result равен null!");
            }
        }

        private long[] ExtractUserId(string comment)
        {
            return Regex.Matches(comment, @"\[USER=(\d+)\]")
        .Select(match => long.Parse(match.Groups[1].Value))
        .ToArray();
        }
    }
}
