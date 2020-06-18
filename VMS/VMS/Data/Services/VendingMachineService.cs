using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Data.Models;
using VMS.Data.Models.DTOs;
using VMS.Data.Models.Responses;
using VMS.Data.Services.Interfaces;

namespace VMS.Data.Services
{
    // wallet = bunch of coins (1 euro, 50 cent, 20 cent, 10 cent)
    public class VendingMachineService : IVendingMachineService
    {
        private Dictionary<Product, int> _productStocks;
        private Dictionary<CoinType, Stack<Coin>> _machineWallet;
        private decimal _depositMoney;
        private IWalletFactory _coinStackFactory;

        public VendingMachineService(IWalletFactory coinStackFactory)
        {
            _coinStackFactory = coinStackFactory;
            _productStocks = _coinStackFactory.GetInitialProductStocks();
            _machineWallet = _coinStackFactory.GetInitialMachineWallet();
            _depositMoney = 0;
        }

        public async Task<ProductDto[]> GetProductsAsync()
        {
            var productDtos = _productStocks.Keys
                .Select(p => new ProductDto() { Name = p.Name, Price = p.Price })
                .ToArray();

            return await Task.FromResult(productDtos);
        }

        public async Task<decimal> GetDepositMoneyAsync()
        {
            return await Task.FromResult(_depositMoney);
        }

        public async Task<InsertCoinResponse> InsertCoinAsync(decimal coin)
        {
            var coinType = coin.ToCoinType();

            _machineWallet[coinType].Push(new Coin(coinType));

            _depositMoney = _depositMoney + coin;

            var response = new InsertCoinResponse()
            {
                IsSuccess = true,
                Message = $"Your coin = {_depositMoney}",
                DepositMoney = _depositMoney
            };

            return await Task.FromResult(response);
        }

        public async Task<CancelOrderResponse> CancelOrderAsync()
        {
            (_, var returnWallet) = GetReturnWallet(_machineWallet, _depositMoney);
            _depositMoney = 0;

            var response = new CancelOrderResponse()
            {
                Wallet = returnWallet.ToWalletDto(),
                Message = $"Order is cancelled.",
                IsSuccess = true
            };

            return await Task.FromResult(response);
        }

        public async Task<BuyProductResponse> BuyProductAsync(string productName)
        {
            var response = new BuyProductResponse();

            // 1. validation: check if product is available
            var productStock = _productStocks
                .Where(p => p.Key.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (!(productStock.Value > 0))
            {
                response = new BuyProductResponse()
                {
                    IsSuccess = false,
                    Message = $"{productName} is out of stock. Please choose other product."
                };

                return await Task.FromResult(response);
            }

            // 2. validation: check if deposit money is sufficient
            if (_depositMoney < productStock.Key.Price)
            {
                response = new BuyProductResponse()
                {
                    IsSuccess = false,
                    Message = $"inserted coin ('{_depositMoney}') is insufficient. Please insert more coin"
                };

                return await Task.FromResult(response);
            }

            // 3. calculate coin changes needed
            var changeVal = Math.Round(_depositMoney - productStock.Key.Price, 2);

            // 4. return coin changes from coin wallet
            (var rest, var returnWallet) = GetReturnWallet(_machineWallet, changeVal);

            // 5. if the amount change is not sufficient, cancel the order
            if (rest > 0)
            {
                // 5.a. put back the coin changes to coin wallet
                RollBackChangesToMachineWallet(_machineWallet, returnWallet);

                response = new BuyProductResponse()
                {
                    IsSuccess = false,
                    Message = "Order is cancelled. Sorry, there is no sufficient coin changes."
                };

                return await Task.FromResult(response);
            }

            // 6. substract product from product stocks
            _productStocks[productStock.Key] = _productStocks[productStock.Key] - 1;

            // 7. reset deposit money
            _depositMoney = 0;

            response.IsSuccess = true;
            response.Message = $"Thank you. You just bought '{productStock.Key.Name}' for '{productStock.Key.Price:F2}' euro. ";
            response.Wallet = returnWallet.ToWalletDto();

            return await Task.FromResult(response);
        }

        private (decimal rest, Dictionary<CoinType, Stack<Coin>>) GetReturnWallet(
                Dictionary<CoinType, Stack<Coin>> machineWallet,
                decimal changeVal)
        {
            var returnWallet = _coinStackFactory.GetEmptyWallet();

            changeVal = ProcessReturnWallet(machineWallet, returnWallet, CoinType.OneEuro, changeVal);
            changeVal = ProcessReturnWallet(machineWallet, returnWallet, CoinType.FiftyCent, changeVal);
            changeVal = ProcessReturnWallet(machineWallet, returnWallet, CoinType.TwentyCent, changeVal);
            changeVal = ProcessReturnWallet(machineWallet, returnWallet, CoinType.TenCent, changeVal);

            return (changeVal, returnWallet);
        }

        private decimal ProcessReturnWallet(
            Dictionary<CoinType, Stack<Coin>> machineWallet,
            Dictionary<CoinType, Stack<Coin>> returnWallet,
            CoinType coinType,
            decimal changeVal)
        {
            var coinTypeVal = coinType.ToDecimal();

            while (changeVal >= coinTypeVal && machineWallet[coinType].Count > 0)
            {
                var fiftyCentCoin = machineWallet[coinType].Pop();
                returnWallet[coinType].Push(fiftyCentCoin);

                changeVal = changeVal - coinTypeVal;
            }

            return changeVal;
        }

        private void RollBackChangesToMachineWallet(
            Dictionary<CoinType, Stack<Coin>> vmCoinStack,
            Dictionary<CoinType, Stack<Coin>> coinChanges)
        {
            foreach (var coinType in coinChanges)
            {
                var coinTypeLength = coinType.Value.Count;
                for (var i = 0; i < coinTypeLength; i++)
                {
                    vmCoinStack[coinType.Key].Push(coinType.Value.Pop());
                }
            }
        }

    }
}
