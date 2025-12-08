using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            //验证注册表单

            //创建user/member
            var user = new AppUser
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.Email,
                Member = new Member
                {
                    DisplayName = registerDto.DisplayName,
                    DateOfBirth = registerDto.DateOfBirth,
                    Gender = registerDto.Gender,
                    Country = registerDto.Country,
                    City = registerDto.City
                }
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("identity", error.Description);
                }
                return ValidationProblem();
            }

            var roleResult = await userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("identity", error.Description);
                }
                return ValidationProblem();
            }

            //注册完视为登录，颁发token
            return await user.UserToDto(tokenService);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //验证用户，密码
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized("Invalid email address or password");
            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) return Unauthorized("Invalid email address or password");
            
            //颁发token
            return await user.UserToDto(tokenService);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            return Ok();
        }
    }
}
