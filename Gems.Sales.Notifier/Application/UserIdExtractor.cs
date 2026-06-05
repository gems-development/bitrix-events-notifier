using System.Text.RegularExpressions;

namespace Gems.Sales.Notifier.Application
{
    internal sealed class UserIdExtractor : IUserIdExtractor
    {
        public long[] ExtractUserIds(string input) {
            if (string.IsNullOrWhiteSpace(input)) return Array.Empty<long>();
            return Regex.Matches(input, @"\[USER=(\d+)\]")
                .Select(match => long.Parse(match.Groups[1].Value))
                .ToArray();
        }
    }
}
