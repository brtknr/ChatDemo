using ChatDemo.API.Models;
using ChatDemo.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatDemo.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        private readonly JWT _jwt;

        public UserController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt.Value;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] AppUserSignUpVM AppUserVM)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = AppUserVM.UserName,
                    Email = AppUserVM.Email
                };
                IdentityResult result = await _userManager.CreateAsync(appUser, AppUserVM.Password);
                if (result.Succeeded)
                    return Ok(result);
                else
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                    return BadRequest(result);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Login(AppUserLoginVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser? user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    //İlgili kullanıcıya dair önceden oluşturulmuş bir Cookie varsa siliyoruz.
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password,true,model.Lock);

                    var isSignedIn = User;

                    if (result.Succeeded) 
                    {
                        // generate token and return
                        AuthenticationModel authModel = await GetTokenAsync(model);
                        return Ok(authModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("NotUser", "Böyle bir kullanıcı bulunmamaktadır.");
                    ModelState.AddModelError("NotUser2", "E-posta veya şifre yanlış.");
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AuthTest()
        {
            return Ok("this user is authorized.");
        }


        private async Task<AuthenticationModel> GetTokenAsync(AppUserLoginVM model)
        {
            AuthenticationModel authenticationModel;
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthenticationModel { IsAuthenticated = false, Message = $"No Accounts Registered with {model.Email}." };

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);

                authenticationModel = new AuthenticationModel
                {
                    IsAuthenticated = true,
                    Message = jwtSecurityToken.ToString(),
                    UserName = user.UserName,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    Email = user.Email,
                };
                return authenticationModel;
            }
            else
            {
                authenticationModel = new AuthenticationModel()
                {
                    IsAuthenticated = false,
                    Message = $"Incorrect Credentials for user {user.Email}."
                };
            }
            return authenticationModel;
        }


        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

    }
}
