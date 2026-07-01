using Gems.Sales.Notifier.Application;

namespace Gems.Sales.Notifier.Tests
{
    public class MessageComposerTest
    {
        [Theory]
        [InlineData(null, 123, "Вы были упомянуты в сделке: https://crm.gemsdev.ru/crm/deal/details/123/")]
        [InlineData("", 123, "Вы были упомянуты в сделке: https://crm.gemsdev.ru/crm/deal/details/123/")]
        [InlineData("Some title", 123, "Вы были упомянуты в сделке Some title: https://crm.gemsdev.ru/crm/deal/details/123/")]
        public void DifInput_ReturnsDifResult(string? title, long dealId, string expectedString)
        {
            var extractor = new NotificationMessageComposer();
            var result = extractor.BuildMessage(title, dealId);
            Assert.Equal(expectedString, result);
        }
    }
}
