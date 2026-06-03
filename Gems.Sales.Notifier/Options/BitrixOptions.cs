namespace Gems.Sales.Notifier.Options
{
    public class BitrixOptions
    {
        public const string SectionName = "BitrixApi";
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
    }
}
