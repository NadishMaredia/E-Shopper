﻿using EShop.Services.ShoppingCartAPI.Models.Dto;
using EShop.Services.ShoppingCartAPI.Services.IService;
using Newtonsoft.Json;

namespace EShop.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var client = httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/Coupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }
            return new CouponDto();
        }
    }
}
