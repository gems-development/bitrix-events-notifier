using System.Text.Json.Serialization;

namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Dto
{
    public class BitrixResponseWrapperDto
    {
        [JsonPropertyName("result")]
        public BitrixTaskDto Result { get; set; }
    }
}
