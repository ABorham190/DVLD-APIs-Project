using dvld_api.Auth;
using dvld_api.Auth.UserDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace dvld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser(NewUserDto newUserDto)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = newUserDto.UserName,
                    Email = newUserDto.Email,
                };

                IdentityResult result=await _userManager.CreateAsync(user,newUserDto.Password);

                if (result.Succeeded)
                {
                    return Ok("Succeeded");
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByNameAsync(loginDto.UserName);
                if (appUser != null)
                {
                    if (await _userManager.CheckPasswordAsync(appUser, loginDto.Password))
                    {
                        var Claims = new List<Claim>();
                        Claims.Add(new Claim(ClaimTypes.Name, appUser.UserName!));
                        Claims.Add(new Claim(ClaimTypes.NameIdentifier, appUser.Id));
                        Claims.Add(new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        var roles = await _userManager.GetRolesAsync(appUser);
                        foreach(var role in roles)
                        {
                            Claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
                        var sc = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            claims: Claims,
                            issuer: _configuration["JWT:Issuer"],
                            signingCredentials: sc,
                            expires: DateTime.Now.AddHours(1),
                            audience: _configuration["Audence"]
                            );

                        var _token = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                        };

                        return Ok(_token);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
            }
           
            
                return BadRequest(ModelState);
        }
    }
}
