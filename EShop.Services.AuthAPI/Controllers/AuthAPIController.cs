using EShop.Services.AuthAPI.IServices;
using EShop.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Services.AuthAPI.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthAPIController : ControllerBase
	{
		private readonly IAuthService authService;
		protected ResponseDto response;

        public AuthAPIController(IAuthService authService)
        {
            this.authService = authService;
			response = new();
        }

        [HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
		{
			var errorMessage = await authService.Register(model);
			if(!string.IsNullOrEmpty(errorMessage))
			{
				response.IsSuccess = false;
				response.Message = errorMessage;
				return BadRequest(response);
			}
			return Ok(response);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
		{
			var loginResponse = await authService.Login(model);
			if(loginResponse.User == null)
			{
				response.IsSuccess = false;
				response.Message = "Username or password is incorrect";
				return BadRequest(response);
			}

			response.Result = loginResponse;
			return Ok(response);
		}

		[HttpPost("assignRole")]
		public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
		{
			var role = await authService.AssignRole(model.Email, model.Role.ToUpper());
			if (!role)
			{
				response.IsSuccess = false;
				response.Message = "Error encountered";
				return BadRequest(response);
			}

			return Ok(response);
		}
	}
}
