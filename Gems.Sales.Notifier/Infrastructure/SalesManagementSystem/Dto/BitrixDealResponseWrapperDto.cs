using System.Text.Json.Serialization;

namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Dto
{
    public class BitrixDealResponseWrapperDto
    {
        [JsonPropertyName("result")]
        public BitrixDealResultDto Result { get; set; } = null!;
    }
    public class BitrixDealResultDto
    {
        [JsonPropertyName("item")]
        public BitrixDealDto Item { get; set; } = null!;
    }

    public class BitrixDealDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
