using EShop.Web.Models;

namespace EShop.Web.Services.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> GetAllOrders(string? userId);
        Task<ResponseDto?> GetOrder(int orderId);
    }
}
