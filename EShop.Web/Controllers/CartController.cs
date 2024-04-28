using EShop.Web.Models;
using EShop.Web.Services;
using EShop.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace EShop.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IOrderService orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            this.cartService = cartService;
            this.orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoForLoggedInUser());
        }

        private async Task<CartDto> LoadCartDtoForLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await cartService.GetCartByUserIdAsync(userId);
            if(response != null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            ResponseDto? response = await cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart update successfully!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto? response = await cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon added successfully!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            ResponseDto? response = await cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return RedirectToAction(nameof(CartIndex));
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;

            var response = await orderService.CreateOrder(cart);

            if(response != null && response.IsSuccess)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(CartIndex));
        }

        private async Task<CartDto> LoadCartBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await cartService.GetCartByUserIdAsync(userId);
            if (response != null & response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }
    }
}
