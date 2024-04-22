using EShop.Web.Models;
using EShop.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EShop.Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService productService;

		public ProductController(IProductService productService)
		{
			this.productService = productService;
		}

		public async Task<IActionResult> ProductIndex()
		{
			List<ProductDto> productList = new();
			ResponseDto? response = await productService.GetAllProductAsync();


			if (response != null && response.IsSuccess) 
			{
				productList = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
				TempData["success"] = "Product(s) fetch successfully";
			}
			else
			{
				TempData["error"] = response?.Message;
			}

			return View(productList);
		}
	}
}
