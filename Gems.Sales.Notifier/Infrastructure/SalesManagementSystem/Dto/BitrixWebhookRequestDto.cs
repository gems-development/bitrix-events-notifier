using System.Text.Json.Serialization;

namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Dto
{
    public class BitrixWebhookRequestDto
    {
        public int[] UserIds { get; set; }

        public class BitrixResponseWrapper
        {
            [JsonPropertyName("result")]
            public BitrixTaskDto Result { get; set; }

            [JsonPropertyName("time")]
            public BitrixTime Time { get; set; }
        }

        public class BitrixTaskDto
        {
            [JsonPropertyName("ID")]
            public string Id { get; set; }

            [JsonPropertyName("ENTITY_ID")]
            public string EntityId { get; set; }

            [JsonPropertyName("ENTITY_TYPE")]
            public string EntityType { get; set; }

            [JsonPropertyName("CREATED")]
            public string Created { get; set; }

            [JsonPropertyName("COMMENT")]
            public string Comment { get; set; }

            [JsonPropertyName("AUTHOR_ID")]
            public string AuthorId { get; set; }
        }

        public class BitrixTime
        {
            [JsonPropertyName("start")]
            public double Start { get; set; }

            [JsonPropertyName("finish")]
            public double Finish { get; set; }

            [JsonPropertyName("duration")]
            public double Duration { get; set; }
        }

        //Dto для лида
        public class Lead
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Name { get; set; }
            public string? SecondName { get; set; }
            public string? LastName { get; set; }
            public string? CompanyTitle { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Description { get; set; }
            public string? Time { get; set; }
        }
        //Dto для компании
        public class Company
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Description { get; set; }
            public string? Time { get; set; }
        }
    }
}
