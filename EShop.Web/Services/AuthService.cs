using EShop.Web.Models;
using EShop.Web.Services.IService;
using EShop.Web.Util;

namespace EShop.Web.Services
{
	public class AuthService : IAuthService
	{
		private readonly IBaseService baseService;
		public AuthService(IBaseService baseService)
		{
			this.baseService = baseService;
		}
		public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.POST,
				Data = registrationRequestDto,
				Url = SD.AuthAPIBase + "/api/auth/assignRole"
			});
		}

		public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.POST,
				Data = loginRequestDto,
				Url = SD.AuthAPIBase + "/api/auth/login"
			}, withBearer:false);
		}

		public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
		{
			return await baseService.SendAsync(new RequestDto()
			{
				ApiType = Util.SD.ApiType.POST,
				Data = registrationRequestDto,
				Url = SD.AuthAPIBase + "/api/auth/register"
			}, withBearer: false);
		}
	}
}
