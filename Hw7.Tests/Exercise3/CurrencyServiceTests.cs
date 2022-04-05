using Hw7.Exercise3;
using Hw7.Exercise3.Abstractions;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Hw7.Tests.Exercise3
{
    public class CurrencyServiceTests
    {
        [Fact]
        public void Get_KnownCurrencies_ReturnsValidRate()
        {
            var storage = GetStorageMock(new Dictionary<string, decimal>
            {
                ["USD"] = 1m,
                ["EUR"] = 1.2m,
                ["BTC"] = 50_000m
            });

            var notifications = GetNotificationsMock();

            var service = new CurrencyService(storage, notifications);
            var result = service.GetCurrencyRate("BTC");

            Assert.Equal(50_000m, result);
        }

        [Fact]
        public void Set_CurrencyRate_UpdatesData()
        {
            var storage = GetStorageMock(new Dictionary<string, decimal>
            {
                ["USD"] = 1m,
                ["EUR"] = 1.2m,
                ["BTC"] = 50_000m
            });

            var notifications = GetNotificationsMock();

            var service = new CurrencyService(storage, notifications);
            service.SetCurrencyRate("BTC", 20_000m);

            storage.Received().UpsertCurrencyRate("BTC", 20_000m);
            notifications.Received().NotifyCurrencyRateChanged("BTC", 20_000m);
        }

        [Fact]
        public void Failed_CurrencyRateUpsert_DontSendNotification()
        {
            var storage = GetStorageMockWithException();
            var notifications = GetNotificationsMock();
            var service = new CurrencyService(storage, notifications);

            var _ = Assert.ThrowsAny<Exception>(() =>
            {
                service.SetCurrencyRate("BTC", 20_000m);
            });

            storage.Received().UpsertCurrencyRate("BTC", 20_000m);
            notifications.DidNotReceiveWithAnyArgs().NotifyCurrencyRateChanged(default!, default);
        }

        [Fact]
        public void Exchange_KnownCurrencies_ReturnsValidResult()
        {
            var storage = GetStorageMock(new Dictionary<string, decimal>
            {
                ["USD"] = 1m,
                ["EUR"] = 1.2m,
                ["BTC"] = 50_000m
            });

            var notifications = GetNotificationsMock();

            var service = new CurrencyService(storage, notifications);
            var result = service.Exchange("EUR", "USD", 10);

            Assert.Equal(12m, result);
        }

        #region Mocks

        private static ICurrencyStorage GetStorageMock(Dictionary<string, decimal> rates)
        {
            // TODO: Method should returns ICurrencyStorage
            // created with nSubstitute only
            // ICurrencyStorage.GetCurrencyRate(...) should returns given rates.
            // More details at https://nsubstitute.github.io/help/getting-started/

            var storage = Substitute.For<ICurrencyStorage>();

            storage
                .When(x => x.GetCurrencyRate(Arg.Any<string>()))
                .Do(x =>
                {
                    foreach (var rate in rates)
                        _ = storage.GetCurrencyRate(Arg.Is(rate.Key)).Returns(rate.Value);
                });

            return storage;
        }

        private static ICurrencyStorage GetStorageMockWithException()
        {
            // TODO: Method should returns ICurrencyStorage
            // created with nSubstitute only.
            // ICurrencyStorage.UpsertCurrencyRate(...) should thrown exception.
            // More details at https://nsubstitute.github.io/help/throwing-exceptions/
            var storage = Substitute.For<ICurrencyStorage>();

            storage
                .When(x => x.UpsertCurrencyRate(Arg.Any<string>(), Arg.Any<decimal>()))
                .Do(x => { throw new ArgumentException(); });

            return storage;
        }

        private static ICurrencyNotifications GetNotificationsMock()
        {
            // TODO: Method should returns ICurrencyNotifications
            // created with nSubstitute only
            // More details at https://nsubstitute.github.io/help/getting-started/
            var notifications = Substitute.For<ICurrencyNotifications>();
            return notifications;
        }
        #endregion
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
