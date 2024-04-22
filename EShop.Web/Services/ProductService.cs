using EShop.Web.Models;
using EShop.Web.Services.IService;
using EShop.Web.Util;

namespace EShop.Web.Services
{
	public class ProductService : IProductService
	{
		private readonly IBaseService _baseService;

		public ProductService(IBaseService baseService)
		{
			_baseService = baseService;
		}

		public async Task<ResponseDto> CreateProduct(ProductDto productDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.POST,
				Url = SD.ProductAPIBase + "/api/product",
				Data = productDto
			});
		}

		public async Task<ResponseDto> DeleteProductById(int productId)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.DELETE,
				Url = SD.ProductAPIBase + "/api/product/" + productId
			});
		}

		public async Task<ResponseDto> GetAllProductAsync()
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.GET,
				Url = SD.ProductAPIBase + "/api/product"
			});
		}

		public async Task<ResponseDto> GetProductByCategoryAsync(string category)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.GET,
				Url = SD.ProductAPIBase + "/api/product/GetByCategory" + category
			});
		}

		public async Task<ResponseDto> GetProductByIdAsync(int productId)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.GET,
				Url = SD.ProductAPIBase + "/api/product/" + productId
			});
		}

		public async Task<ResponseDto> UpdateProduct(ProductDto productDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.PUT,
				Url = SD.ProductAPIBase + "/api/product",
				Data = productDto
			});
		}
	}
}
