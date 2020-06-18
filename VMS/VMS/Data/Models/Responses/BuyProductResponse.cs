using VMS.Data.Models.DTOs;

namespace VMS.Data.Models.Responses
{
    public class BuyProductResponse : BaseResponse
    {
        public WalletDto Wallet;

        public BuyProductResponse()
        {
            Wallet = new WalletDto();
        }
    }
}
