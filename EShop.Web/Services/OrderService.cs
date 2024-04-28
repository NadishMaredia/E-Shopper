using EShop.Web.Models;
using EShop.Web.Services.IService;
using EShop.Web.Util;

namespace EShop.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService baseService;

        public OrderService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = Util.SD.ApiType.POST,
                Data = cartDto,
                Url = SD.OrderAPIBase +"/api/order/CreateOrder"
            });
        }

        public async Task<ResponseDto?> GetAllOrders(string? userId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = Util.SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrders?userId="+ userId
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = Util.SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrder/" + orderId
            });
        }
    }
}
