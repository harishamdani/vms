using System.Collections.Generic;
using VMS.Data.Models;

namespace VMS.Data.Services.Interfaces
{
    public interface IWalletFactory
    {
        Dictionary<CoinType, Stack<Coin>> GetEmptyWallet();
        Dictionary<CoinType, Stack<Coin>> GetInitialMachineWallet();
        Dictionary<Product, int> GetInitialProductStocks();
    }
}