using System.Threading.Tasks;
using VMS.Data.Models.DTOs;
using VMS.Data.Models.Responses;

namespace VMS.Data.Services.Interfaces
{
    public interface IVendingMachineService
    {
        Task<BuyProductResponse> BuyProductAsync(string productName);
        Task<CancelOrderResponse> CancelOrderAsync();
        Task<decimal> GetDepositMoneyAsync();
        Task<ProductDto[]> GetProductsAsync();
        Task<InsertCoinResponse> InsertCoinAsync(decimal coin);
    }
}