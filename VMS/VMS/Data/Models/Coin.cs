using VMS.Data.Services;

namespace VMS.Data.Models
{
    public class Coin
    {
        public Coin()
        {

        }

        public Coin(CoinType coinType)
        {
            Name = coinType;
            Value = coinType.ToDecimal();
        }

        public CoinType Name { get; set; }

        public decimal Value { get; set; }
    }


}
