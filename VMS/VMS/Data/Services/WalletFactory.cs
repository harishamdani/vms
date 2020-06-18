using System.Collections.Generic;
using VMS.Data.Models;
using VMS.Data.Services.Interfaces;

namespace VMS.Data.Services
{
    public class WalletFactory : IWalletFactory
    {
        private int _oneEuroAmount;
        private int _fiftyCentAmount;
        private int _twentyCentAmount;
        private int _tenCentAmount;

        public WalletFactory()
        {
            _oneEuroAmount = 100;
            _fiftyCentAmount = 100;
            _twentyCentAmount = 100;
            _tenCentAmount = 100;
        }

        public WalletFactory(
                int oneEuroAmount,
                int fiftyCentAmount,
                int twentyCentAmount,
                int tenCentAmount
            )
        {
            _oneEuroAmount = oneEuroAmount;
            _fiftyCentAmount = fiftyCentAmount;
            _twentyCentAmount = twentyCentAmount;
            _tenCentAmount = tenCentAmount;
        }

        public Dictionary<CoinType, Stack<Coin>> GetInitialMachineWallet()
        {


            var oneEuroStacks = (_oneEuroAmount > 0) ? PushCoinsToStack(CoinType.OneEuro, _oneEuroAmount) : new Stack<Coin>();
            var fiftyCentStacks = (_fiftyCentAmount > 0) ? PushCoinsToStack(CoinType.FiftyCent, _fiftyCentAmount) : new Stack<Coin>();
            var twentyCentStacks = (_twentyCentAmount > 0) ? PushCoinsToStack(CoinType.TwentyCent, _twentyCentAmount) : new Stack<Coin>();
            var tencentStacks = (_tenCentAmount > 0) ? PushCoinsToStack(CoinType.TenCent, _tenCentAmount) : new Stack<Coin>();

            var coinStacks = new Dictionary<CoinType, Stack<Coin>>
            {
                { CoinType.OneEuro, oneEuroStacks },
                { CoinType.FiftyCent, fiftyCentStacks },
                { CoinType.TwentyCent, twentyCentStacks },
                { CoinType.TenCent, tencentStacks }
            };

            return coinStacks;
        }

        private static Stack<Coin> PushCoinsToStack(CoinType coinType, int amount)
        {
            var stack = new Stack<Coin>();

            for (var i = 0; i < amount; i++)
            {
                stack.Push(new Coin(coinType));
            }

            return stack;
        }

        public Dictionary<CoinType, Stack<Coin>> GetEmptyWallet()
        {
            return new Dictionary<CoinType, Stack<Coin>>()
            {
                { CoinType.OneEuro, new Stack<Coin>()},
                { CoinType.FiftyCent, new Stack<Coin>()},
                { CoinType.TwentyCent, new Stack<Coin>()},
                { CoinType.TenCent, new Stack<Coin>()},
            };
        }

        public Dictionary<Product, int> GetInitialProductStocks()
        {
            return new Dictionary<Product, int>()
            {
                {new Product(){Name= "Tea", Price = 1.30M }, 10},
                {new Product(){Name= "Espresso", Price = 1.80M }, 20},
                {new Product(){Name= "Juice", Price = 1.80M }, 20},
                {new Product(){Name= "Chicken Soup", Price = 1.80M }, 15},
            };
        }
    }
}
