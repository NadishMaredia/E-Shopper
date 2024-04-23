using AutoMapper;
using EShop.Services.ShoppingCartAPI.Data;
using EShop.Services.ShoppingCartAPI.Models;
using EShop.Services.ShoppingCartAPI.Models.Dto;
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

        public CartAPIController(IMapper mapper, AppDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
            responseDto = new ResponseDto();
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
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
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

                foreach(var item in cartDto.CartDetails)
                {
                    cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
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
    }
}
