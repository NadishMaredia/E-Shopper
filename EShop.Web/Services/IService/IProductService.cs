using EShop.Web.Models;

namespace EShop.Web.Services.IService
{
	public interface IProductService
	{
		Task<ResponseDto> GetProductByCategoryAsync(string category);
		Task<ResponseDto> GetProductByIdAsync(int productId);
		Task<ResponseDto> GetAllProductAsync();
		Task<ResponseDto> CreateProduct(ProductDto productDto);
		Task<ResponseDto> UpdateProduct(ProductDto productDto);
		Task<ResponseDto> DeleteProductById(int productId);
	}
}
