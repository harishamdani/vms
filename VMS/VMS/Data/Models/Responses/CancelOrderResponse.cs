using VMS.Data.Models.DTOs;

namespace VMS.Data.Models.Responses
{
    public class CancelOrderResponse : BaseResponse
    {
        public WalletDto Wallet { get; set; }

        public CancelOrderResponse()
        {
            Wallet = new WalletDto();
        }
    }
}
