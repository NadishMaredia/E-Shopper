using EShop.Services.ShoppingCartAPI.Models;

namespace EShop.Services.ShoppingCartAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
