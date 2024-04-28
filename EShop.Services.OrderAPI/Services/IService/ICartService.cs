
namespace EShop.Services.OrderAPI.Services.IService
{
    public interface ICartService
    {
        Task<bool> UpdateCartStatus(int cartHeaderId);
    }
}
