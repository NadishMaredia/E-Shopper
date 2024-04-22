using EShop.Web.Models;
using EShop.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

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

		public async Task<IActionResult> ProductCreate()
		{
			return View();
		}
		[HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto productDto)
        {
			if(ModelState.IsValid)
			{
				ResponseDto? response = await productService.CreateProduct(productDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDto);
        }

		[HttpGet]
		public async Task<IActionResult> ProductDelete(int productId)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? response = await productService.DeleteProductById(productId);

				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Product deleted successfully";
				}
				else
				{
					TempData["error"] = response?.Message;
				}
			}
			return RedirectToAction(nameof(ProductIndex));
		}

		public async Task<IActionResult> ProductEdit(int productId)
		{
			ResponseDto? response = await productService.GetProductByIdAsync(productId);

			if(response != null && response.IsSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{
                TempData["error"] = response?.Message;
            }
			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> ProductEdit(ProductDto productDto)
		{
            if (ModelState.IsValid)
            {
				ResponseDto? response = await productService.UpdateProduct(productDto);

				if(response != null && response.IsSuccess)
				{
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(productDto);
        }

	}
}
