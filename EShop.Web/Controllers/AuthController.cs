using EShop.Web.Models;
using EShop.Web.Services.IService;
using EShop.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace EShop.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
			this.authService = authService;
        }
		[HttpGet]
        public IActionResult Login()
		{
			LoginRequestDto loginRequestDto = new();
			return View(loginRequestDto);
		}
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto result = await authService.LoginAsync(obj);

            if (result != null && result.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(result.Result));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = result?.Message;
                ModelState.AddModelError("CustomError", result.Message);
                return View(obj);
            }
        }

        [HttpGet]
		public IActionResult Register()
		{
			var roleList = new List<SelectListItem>()
			{
				new SelectListItem{ Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem{ Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };

			ViewBag.RoleList = roleList;
			return View();
		}

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
			ResponseDto result = await authService.RegisterAsync(obj);
			ResponseDto assignRole;

			if(result != null && result.IsSuccess)
			{
				if(string.IsNullOrEmpty(obj.Role))
				{
					obj.Role = SD.RoleCustomer;
				}
				assignRole = await authService.AssignRoleAsync(obj);
				if(assignRole != null && assignRole.IsSuccess)
				{
					TempData["success"] = "Registration successful";
					return RedirectToAction(nameof(Login));

				}
			} else
			{
                TempData["error"] = result?.Message;
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{ Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem{ Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };

            ViewBag.RoleList = roleList;

            return View(obj);
        }
        [HttpGet]
        public IActionResult logout()
        {
            return View();
        }
    }
}
