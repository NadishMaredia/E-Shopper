using AutoMapper;
using Azure;
using EShop.Services.OrderAPI.Data;
using EShop.Services.OrderAPI.Models;
using EShop.Services.OrderAPI.Models.Dto;
using EShop.Services.OrderAPI.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto responseDto;
        private readonly AppDbContext db;
        private IMapper mapper;

        public OrderAPIController(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            responseDto = new ResponseDto();
            this.mapper = mapper;
        }

        [HttpGet]
        public ResponseDto? Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                if(User.IsInRole(SD.RoleAdmin))
                {
                    objList = db.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();
                } else
                {
                    objList = db.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                responseDto.Result = mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
                responseDto.Message = "Orders fetch successfully!";
            }
            catch(Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = db.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);
                responseDto.Result = mapper.Map<OrderHeaderDto>(orderHeader);
            } 
            catch(Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }

        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);
                orderHeaderDto.OrderTotal = Math.Round(cartDto.CartHeader.CartTotal, 2);

                OrderHeader orderCreated = db.OrderHeaders.Add(mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                responseDto.Result = orderHeaderDto;
            }
            catch(Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return responseDto;
        }
    }
}
