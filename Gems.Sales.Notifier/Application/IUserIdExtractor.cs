namespace Gems.Sales.Notifier.Application
{
    public interface IUserIdExtractor
    {
        long[] ExtractUserIds(string input);
    }
}