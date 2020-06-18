using NUnit.Framework;
using VMS.Data.Services;
using VMS.Data.Services.Interfaces;

namespace VMS.Test
{
    [TestFixture]
    public class VendingMachineServiceTests
    {

        private IVendingMachineService _vendingMachineService;
        private IWalletFactory _walletFactory;

        [OneTimeSetUp]
        public void FixtureSetup()
        {

        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {

        }

        [SetUp]
        public void Setup()
        {
            _walletFactory = new WalletFactory();
            _vendingMachineService = new VendingMachineService(_walletFactory);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [TestCase(1.4, "Tea", 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1.5, "Tea", 0, 0, 0, 1, 0, 0, 0, 0)]
        public void BuyProduct_NoCoinChangeAvailable_ShouldRevertOrder(
                decimal deposit,
                string productName,
                int initialOneEuroAmount,
                int initialFiftyCentAmount,
                int initialTwentyCentAmount,
                int initialTenCentAmount,                
                int expectedOneEuroCoinAmount,
                int expectedFiftyCentCoinAmount,
                int expectedTwentyCentCoinAmount,
                int expectedTenCentCoinAmount)
        {
            // arrange
            _walletFactory = new WalletFactory(
                    initialOneEuroAmount, 
                    initialFiftyCentAmount,
                    initialTwentyCentAmount,
                    initialTenCentAmount
                );

            _vendingMachineService = new VendingMachineService(_walletFactory);

            _vendingMachineService.InsertCoinsFromTestInput(deposit.ToWalletDto());

            // action
            var result = _vendingMachineService.BuyProductAsync(productName).Result;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Order is cancelled. Sorry, there is no sufficient coin changes."));
            Assert.That(result.Wallet, Is.Not.Null);
            Assert.That(result.Wallet.TotalValue, Is.EqualTo(0));
            Assert.That(result.Wallet.OneEuroCoinAmount, Is.EqualTo(expectedOneEuroCoinAmount));
            Assert.That(result.Wallet.FiftyCentCoinAmount, Is.EqualTo(expectedFiftyCentCoinAmount));
            Assert.That(result.Wallet.TwentyCentCoinAmount, Is.EqualTo(expectedTwentyCentCoinAmount));
            Assert.That(result.Wallet.TenCentCoinAmount, Is.EqualTo(expectedTenCentCoinAmount));
        }

        [TestCase(2.3, "Tea", 1, 0, 100, 100, 100, 1, 0, 0, 0)]
        [TestCase(2.1, "Tea", .8, 0, 0, 100, 100, 0, 0, 4, 0)]
        [TestCase(1.8, "Tea", .5, 0, 0, 0, 100, 0, 1, 0, 0)]
        [TestCase(2.2, "Tea", .9, 0, 0, 100, 100, 0, 0, 4, 1)]
        [TestCase(2.2, "Tea", .9, 0, 0, 1, 100, 0, 0, 2, 5)]
        public void BuyProduct_SomeCoinsAreEmpty_ShouldReturn_OtherCoinChanges(
                decimal deposit,
                string productName,
                decimal expectedReturnVal,
                int initialOneEuroAmount,
                int initialFiftyCentAmount,
                int initialTwentyCentAmount,
                int initialTenCentAmount,
                int expectedOneEuroCoinAmount,
                int expectedFiftyCentCoinAmount,
                int expectedTwentyCentCoinAmount,
                int expectedTenCentCoinAmount
            )
        {
            // arrange
            _walletFactory = new WalletFactory(
                    initialOneEuroAmount, 
                    initialFiftyCentAmount,
                    initialTwentyCentAmount,
                    initialTenCentAmount
                );

            _vendingMachineService = new VendingMachineService(_walletFactory);

            _vendingMachineService.InsertCoinsFromTestInput(deposit.ToWalletDto());

            // action
            var result = _vendingMachineService.BuyProductAsync(productName).Result;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Wallet, Is.Not.Null);
            Assert.That(result.Wallet.TotalValue, Is.EqualTo(expectedReturnVal));
            Assert.That(result.Wallet.OneEuroCoinAmount, Is.EqualTo(expectedOneEuroCoinAmount));
            Assert.That(result.Wallet.FiftyCentCoinAmount, Is.EqualTo(expectedFiftyCentCoinAmount));
            Assert.That(result.Wallet.TwentyCentCoinAmount, Is.EqualTo(expectedTwentyCentCoinAmount));
            Assert.That(result.Wallet.TenCentCoinAmount, Is.EqualTo(expectedTenCentCoinAmount));

        }

        [TestCase(1.3, "Tea", 0, 0, 0, 0, 0)]
        [TestCase(2.3, "Tea", 1, 1, 0, 0, 0)]
        [TestCase(2.8, "Tea", 1.5, 1, 1, 0, 0)]
        [TestCase(3.0, "Tea", 1.7, 1, 1, 1, 0)]
        [TestCase(3.1, "Tea", 1.8, 1, 1, 1, 1)]
        [TestCase(2.2, "Tea", 0.9, 0, 1, 2, 0)]
        [TestCase(1.6, "Tea", 0.3, 0, 0, 1, 1)]
        [TestCase(1.8, "Espresso", 0, 0, 0, 0, 0)]
        [TestCase(2.8, "Espresso", 1, 1, 0, 0, 0)]
        [TestCase(3.3, "Espresso", 1.5, 1, 1, 0, 0)]
        [TestCase(3.5, "Espresso", 1.7, 1, 1, 1, 0)]
        [TestCase(3.6, "Espresso", 1.8, 1, 1, 1, 1)]
        [TestCase(2.7, "Espresso", 0.9, 0, 1, 2, 0)]
        [TestCase(2.1, "Espresso", 0.3, 0, 0, 1, 1)]
        [TestCase(1.8, "Juice", 0, 0, 0, 0, 0)]
        [TestCase(2.8, "Juice", 1, 1, 0, 0, 0)]
        [TestCase(3.3, "Juice", 1.5, 1, 1, 0, 0)]
        [TestCase(3.5, "Juice", 1.7, 1, 1, 1, 0)]
        [TestCase(3.6, "Juice", 1.8, 1, 1, 1, 1)]
        [TestCase(2.7, "Juice", 0.9, 0, 1, 2, 0)]
        [TestCase(2.1, "Juice", 0.3, 0, 0, 1, 1)]
        [TestCase(1.8, "Chicken Soup", 0, 0, 0, 0, 0)]
        [TestCase(2.8, "Chicken Soup", 1, 1, 0, 0, 0)]
        [TestCase(3.3, "Chicken Soup", 1.5, 1, 1, 0, 0)]
        [TestCase(3.5, "Chicken Soup", 1.7, 1, 1, 1, 0)]
        [TestCase(3.6, "Chicken Soup", 1.8, 1, 1, 1, 1)]
        [TestCase(2.7, "Chicken Soup", 0.9, 0, 1, 2, 0)]
        [TestCase(2.1, "Chicken Soup", 0.3, 0, 0, 1, 1)]
        public void BuyProduct_ShouldReturn_CoinChanges(
                decimal deposit,
                string productName,
                decimal expectedReturnVal,
                int expectedOneEuroCoinAmount,
                int expectedFiftyCentCoinAmount,
                int expectedTwentyCentCoinAmount,
                int expectedTenCentCoinAmount
            )
        {
            // arrange
            _vendingMachineService.InsertCoinsFromTestInput(deposit.ToWalletDto());

            // action
            var result = _vendingMachineService.BuyProductAsync(productName).Result;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Wallet, Is.Not.Null);
            Assert.That(result.Wallet.TotalValue, Is.EqualTo(expectedReturnVal));
            Assert.That(result.Wallet.OneEuroCoinAmount, Is.EqualTo(expectedOneEuroCoinAmount));
            Assert.That(result.Wallet.FiftyCentCoinAmount, Is.EqualTo(expectedFiftyCentCoinAmount));
            Assert.That(result.Wallet.TwentyCentCoinAmount, Is.EqualTo(expectedTwentyCentCoinAmount));
            Assert.That(result.Wallet.TenCentCoinAmount, Is.EqualTo(expectedTenCentCoinAmount));

        }

    }
}
