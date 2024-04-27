using AutoMapper;
using EShop.Services.ShoppingCartAPI.Data;
using EShop.Services.ShoppingCartAPI.Models;
using EShop.Services.ShoppingCartAPI.Models.Dto;
using EShop.Services.ShoppingCartAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace EShop.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto responseDto;
        private IMapper mapper;
        private readonly AppDbContext db;
        private readonly IProductService productService;
        private readonly ICouponService couponService;

        public CartAPIController(IMapper mapper, AppDbContext db, IProductService productService, ICouponService couponService)
        {
            this.mapper = mapper;
            this.db = db;
            responseDto = new ResponseDto();
            this.productService = productService;
            this.couponService = couponService;
        }
        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if(cartHeaderFromDb == null)
                {
                    CartHeader cartHeader = mapper.Map<CartHeader>(cartDto.CartHeader);
                    db.CartHeaders.Add(cartHeader);
                    await db.SaveChangesAsync();

                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    db.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await db.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await db.CartDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.First().ProductId 
                        && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if(cartDetailsFromDb == null)
                    {
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        db.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await db.SaveChangesAsync();
                    } 
                    else
                    {
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDeatilsId = cartDetailsFromDb.CartDeatilsId;
                        db.CartDetails.Update(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await db.SaveChangesAsync();
                    }
                }
                responseDto.Result = cartDto;
            } 
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = db.CartDetails.First(u => u.CartDeatilsId == cartDetailsId);

                int totalCount = db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                db.CartDetails.Remove(cartDetails);

                if(totalCount == 1)
                {
                    var cartHeaderToRemove = await db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await db.SaveChangesAsync();

                responseDto.Result = true;
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new()
                {
                    CartHeader = mapper.Map<CartHeaderDto>(db.CartHeaders.First(u => u.UserId == userId))
                };

                cartDto.CartDetails = mapper.Map<IEnumerable<CartDetailsDto>>(db.CartDetails.Where(u => u.CartHeaderId == cartDto.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await productService.GetProducts();

                foreach(var item in cartDto.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                if(!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto coupon = await couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                    if(coupon != null && cartDto.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cartDto.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = db.CartHeaders.First(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                db.CartHeaders.Update(cartFromDb);
                await db.SaveChangesAsync();
                responseDto.Result = true;
                responseDto.Message = "Coupon code applied";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = db.CartHeaders.First(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                db.CartHeaders.Update(cartFromDb);
                await db.SaveChangesAsync();
                responseDto.Result = true;
                responseDto.Message = "Coupon code removed";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

    }
}
