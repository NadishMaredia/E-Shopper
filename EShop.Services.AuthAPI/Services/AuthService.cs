using EShop.Services.AuthAPI.Data;
using EShop.Services.AuthAPI.IServices;
using EShop.Services.AuthAPI.Models;
using EShop.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace EShop.Services.AuthAPI.Services
{
	public class AuthService : IAuthService
	{
		private readonly AppDbContext db;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
			this.db = db;
			this.userManager = userManager;
			this.roleManager = roleManager;
			_jwtTokenGenerator = jwtTokenGenerator;
        }

		public async Task<bool> AssignRole(string email, string roleName)
		{
			var user = db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
			if (user != null)
			{
				if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
				{
					roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
				}
				await userManager.AddToRoleAsync(user, roleName);
				return true;
			}
			return false;
		}

		public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
		{
			var user = db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
			bool isValid = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
			if(user == null || isValid == false)
			{
				return new LoginResponseDto()
				{
					User = null,
					Token = ""
				};
			}

			var token = _jwtTokenGenerator.GenerateToken(user);

			UserDto userDto = new()
			{
				Email = user.Email,
				ID = user.Id,
				Name = user.Name,
				PhoneNumber = user.PhoneNumber
			};

			LoginResponseDto loginResponseDto = new LoginResponseDto()
			{
				User = userDto,
				Token = token
			};

			return loginResponseDto;
		}

		public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
		{
			ApplicationUser user = new()
			{
				UserName = registrationRequestDto.Email,
				Email = registrationRequestDto.Email,
				NormalizedEmail = registrationRequestDto.Email.ToUpper(),
				Name = registrationRequestDto.Name,
				PhoneNumber = registrationRequestDto.PhoneNumber
			};

			try
			{
				var result = await userManager.CreateAsync(user, registrationRequestDto.Password);
				if(result.Succeeded)
				{
					var userToReturn = db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);
					UserDto userDto = new()
					{
						Email = userToReturn.Email,
						ID = userToReturn.Id,
						Name = userToReturn.Name,
						PhoneNumber = userToReturn.PhoneNumber
					};

					return "";
				}
				else
				{
					return result.Errors.FirstOrDefault().Description;
				}
			}
			catch (Exception ex)
			{

			}

			return "Error Found";
		}
	}
}
