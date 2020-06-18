using VMS.Data.Models;
using VMS.Data.Models.DTOs;
using VMS.Data.Services;
using VMS.Data.Services.Interfaces;

namespace VMS.Test
{
    public static class Helper
    {
        public static void InsertCoinsFromTestInput(
            this IVendingMachineService service,
            WalletDto wallet)
        {
            if (wallet.OneEuroCoinAmount > 0) service.InsertCoins(CoinType.OneEuro, wallet.OneEuroCoinAmount);
            if (wallet.FiftyCentCoinAmount > 0) service.InsertCoins(CoinType.FiftyCent, wallet.FiftyCentCoinAmount);
            if (wallet.TwentyCentCoinAmount > 0) service.InsertCoins(CoinType.TwentyCent, wallet.TwentyCentCoinAmount);
            if (wallet.TenCentCoinAmount > 0) service.InsertCoins(CoinType.TenCent, wallet.TenCentCoinAmount);
        }

        public static void InsertCoins(
                this IVendingMachineService service,
                CoinType coinType,
                int numberOfCoins
            )
        {
            for (var i = 0; i < numberOfCoins; i++)
            {
                service.InsertCoinAsync(coinType.ToDecimal());
            }
        }

        public static WalletDto ToWalletDto(this decimal val)
        {
            var wallet = new WalletDto();

            (val, wallet.OneEuroCoinAmount) = val.GetCoinChange(CoinType.OneEuro);
            (val, wallet.FiftyCentCoinAmount) = val.GetCoinChange(CoinType.FiftyCent);
            (val, wallet.TwentyCentCoinAmount) = val.GetCoinChange(CoinType.TwentyCent);
            (_, wallet.TenCentCoinAmount) = val.GetCoinChange(CoinType.TenCent);

            return wallet;
        }

        private static (decimal rest, int coinAmount) GetCoinChange(
            this decimal val,
            CoinType coinType)
        {
            var coinTypeVal = coinType.ToDecimal();
            var coins = 0;


            while (val >= coinTypeVal)
            {
                coins = coins + 1;
                val = val - coinTypeVal;
            }

            return (val, coins);
        }

    }
}
