using AutoMapper;
using EShop.Services.CouponAPI.Data;
using EShop.Services.CouponAPI.Models;
using EShop.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Services.CouponAPI.Controllers
{
    [Route("api/Coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext db;
        private ResponseDto response;
        private IMapper mapper;

        public CouponAPIController(AppDbContext db, IMapper mapper)
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
                IEnumerable<Coupon> objList = db.Coupons.ToList();
                response.Result = mapper.Map<IEnumerable<CouponDto>>(objList);

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
                Coupon obj = db.Coupons.First(u => u.CouponId == id);
                response.Result = mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto Get(string code)
        {
            try
            {
                Coupon obj = db.Coupons.First(u => u.CouponCode.ToLower() == code.ToLower());
                response.Result = mapper.Map<CouponDto>(obj);
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
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = mapper.Map<Coupon>(couponDto);
                db.Coupons.Add(coupon);
                db.SaveChanges();
                response.Result = mapper.Map<CouponDto>(coupon);
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
		public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = mapper.Map<Coupon>(couponDto);
                db.Coupons.Update(coupon);
                db.SaveChanges();
                response.Result = mapper.Map<CouponDto>(coupon);
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
                Coupon coupon = db.Coupons.First(u => u.CouponId == id);
                db.Coupons.Remove(coupon);
                db.SaveChanges();
                response.Result = mapper.Map<CouponDto>(coupon);
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
