using EShop.Web.Models;
using EShop.Web.Services.IService;
using EShop.Web.Util;

namespace EShop.Web.Services
{
    public class CartService : ICartService
    {
        private readonly IBaseService baseService;
        public CartService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = Util.SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon"
            });
        }

        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = Util.SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/" +userId
            });
        }

        public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = Util.SD.ApiType.POST,
                Data = cartDetailsId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart"
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = Util.SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/CartUpsert"
            });
        }
    }
}
