using EShop.Web.Models;
using EShop.Web.Services.IService;
using EShop.Web.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EShop.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService authService;
        private readonly ITokenProvider tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            this.authService = authService;
            this.tokenProvider = tokenProvider;
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

                await SignInUser(loginResponseDto);

                tokenProvider.SetToken(loginResponseDto.Token);

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

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
