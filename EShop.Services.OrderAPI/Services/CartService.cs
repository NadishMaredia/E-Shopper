
using EShop.Services.OrderAPI.Models.Dto;
using EShop.Services.OrderAPI.Services.IService;
using Newtonsoft.Json;

namespace EShop.Services.OrderAPI.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CartService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<bool> UpdateCartStatus(int cartHeaderId)
        {
            var client = httpClientFactory.CreateClient("Cart");
            var response = await client.GetAsync($"/api/cart/UpdateCartStatus/"+ cartHeaderId);
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return true;
            }
            return false;
        }
    }
}
