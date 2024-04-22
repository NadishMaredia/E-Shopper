using AutoMapper;
using EShop.Services.ProductAPI.Data;
using EShop.Services.ProductAPI.Models;
using EShop.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Services.ProductAPI.Controllers
{
	[Route("api/product")]
	[ApiController]
	public class ProductAPIController : ControllerBase
	{
		private readonly AppDbContext db;
		private ResponseDto response;
		private IMapper mapper;

		public ProductAPIController(AppDbContext db, IMapper mapper)
		{
			this.db = db;
			response = new ResponseDto();
			this.mapper = mapper;
		}

		[HttpGet]
		public ResponseDto Get()
		{
			try
			{
				IEnumerable<Product> productsList = db.Products.ToList();
				response.Result = mapper.Map<IEnumerable<ProductDto>>(productsList);
				response.IsSuccess = true;
				response.Message = "Products fetch successfully!";
			} catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}

			return response;
		}

		[HttpGet]
		[Route("{id:int}")]
		public ResponseDto Get(int id)
		{
			try
			{
				Product obj = db.Products.First(p => p.ProductId == id);
				response.Result = mapper.Map<ProductDto> (obj);
				response.IsSuccess = true;
				response.Message = "Product fetch successfully!";
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}

			return response;
		}

		[HttpGet]
		[Route("GetByCategory/{category}")]
		public ResponseDto Get(string category)
		{
			try
			{
				Product obj = db.Products.First(p => p.CategoryName.ToLower() == category.ToLower());
				response.Result = mapper.Map<ProductDto>(obj);
				response.IsSuccess = true;
				response.Message = "Product fetch successfully!";
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}

			return response;
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Post([FromBody] ProductDto productDto)
		{
			try
			{
				Product product = mapper.Map<Product>(productDto);
				db.Products.Add(product);
				db.SaveChanges();
				response.Result = mapper.Map<ProductDto>(product);
				response.IsSuccess = true;
				response.Message = "Product added successfully!";
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}

			return response;
		}

		[HttpPut]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Put([FromBody] ProductDto productDto)
		{
			try
			{
				Product product = mapper.Map<Product>(productDto);
				db.Products.Update(product);
				db.SaveChanges();
				response.Result = mapper.Map<ProductDto>(product);
				response.IsSuccess = true;
				response.Message = "Product updated successfully!";
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}

			return response;
		}

		[HttpDelete]
		[Route("{id:int}")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Delete(int id)
		{
			try
			{
				Product product = db.Products.First(u => u.ProductId == id);
				db.Products.Remove(product);
				db.SaveChanges();
				response.Result = mapper.Map<Product>(product);
				response.IsSuccess = true;
				response.Message = "Product delete successfully!";
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}

			return response;
		}
	}
}
