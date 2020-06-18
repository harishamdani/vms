using System;
using System.Collections.Generic;
using VMS.Data.Models;
using VMS.Data.Models.DTOs;

namespace VMS.Data.Services
{
    public static class Helper
    {
        public static decimal ToDecimal(this CoinType coin)
        {
            switch (coin)
            {
                case CoinType.OneEuro:
                    return 1M;
                case CoinType.FiftyCent:
                    return 0.5M;
                case CoinType.TwentyCent:
                    return 0.2M;
                case CoinType.TenCent:
                    return 0.1M;
                default:
                    throw new InvalidOperationException("Coin is unknown.");
            }

        }

        public static CoinType ToCoinType(this decimal coin)
        {
            switch (coin)
            {
                case 1M:
                    return CoinType.OneEuro;
                case 0.5M:
                    return CoinType.FiftyCent;
                case 0.2M:
                    return CoinType.TwentyCent;
                case 0.1M:
                    return CoinType.TenCent;
                default:
                    throw new InvalidOperationException("There is no coin type for the given value.");
            }

        }

        public static WalletDto ToWalletDto(this Dictionary<CoinType, Stack<Coin>> coins)
        {
            var total = CoinType.OneEuro.ToDecimal() * coins[CoinType.OneEuro].Count
                        + CoinType.FiftyCent.ToDecimal() * coins[CoinType.FiftyCent].Count
                        + CoinType.TwentyCent.ToDecimal() * coins[CoinType.TwentyCent].Count
                        + CoinType.TenCent.ToDecimal() * coins[CoinType.TenCent].Count;

            return new WalletDto()
            {
                OneEuroCoinAmount = coins[CoinType.OneEuro].Count,
                FiftyCentCoinAmount = coins[CoinType.FiftyCent].Count,
                TwentyCentCoinAmount = coins[CoinType.TwentyCent].Count,
                TenCentCoinAmount = coins[CoinType.TenCent].Count,
                TotalValue = total
            };
        }
    }
}
