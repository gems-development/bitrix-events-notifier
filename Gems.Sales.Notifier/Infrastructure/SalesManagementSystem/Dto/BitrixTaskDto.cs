using System.Text.Json.Serialization;

namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Dto
{

    public class BitrixTaskDto
    {
        [JsonPropertyName("ID")]
        public string Id { get; set; }

        [JsonPropertyName("COMMENT")]
        public string Comment { get; set; }
    }
}
