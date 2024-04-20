using EShop.Services.AuthAPI.Models;

namespace EShop.Services.AuthAPI.IServices
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(ApplicationUser applicationUser);
	}
}
