using Gems.Sales.Notifier.Application;

namespace Gems.Sales.Notifier.Tests
{
    public class UserIdExtractorTest
    {
        [Theory]
        [InlineData("", 0)]
        [InlineData("Word before [USER=6] word after", 1)]
        [InlineData("Word before [USER=6] word between [USER=5] word after", 2)]
        public void EmptyInput_ReturnsEmptyResult(string input, int expectedIdCount)
        {
            var extractor = new UserIdExtractor();
            var result = extractor.ExtractUserIds(input);
            Assert.Equal(expectedIdCount, result.Length);
        }
    }
}
