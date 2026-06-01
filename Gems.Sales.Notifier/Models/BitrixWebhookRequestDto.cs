namespace Gems.Sales.Notifier.Models
{
    public class BitrixWebhookRequestDto
    {
        public int[] UserIds { get; set; }
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
