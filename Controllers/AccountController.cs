using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models;
using Forum.Services;
using Forum.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly ForumDbContext _forumDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenFactory _tokenGenerator;
        public AccountController(ForumDbContext context,
            UserManager<AppUser> userManger,
            ITokenFactory access,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManger;
            _forumDbContext = context;
            _tokenGenerator = access;
            _signInManager = signInManager;
      
        }

    
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync([FromForm]AppUserRegisterPost user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
                //return BadRequest(JsonParser.Parse(ModelState));
            var appuser = new AppUser()
            {
                Email = user.Email,
                UserName = user.Username,
            };
            var createResult = await _userManager.CreateAsync(appuser, user.Password);
            if (!createResult.Succeeded) {
                return BadRequest(createResult.Errors);
            }
            await _userManager.AddToRoleAsync(appuser, "NormalUser");
            var roles = _userManager.GetRolesAsync(appuser);
            appuser = _forumDbContext.Users.Include(x => x.RefreshTokens)
                .FirstOrDefault(x => x.Email == appuser.Email && x.UserName == appuser.UserName);
            var response = _tokenGenerator.StandardRefreshToken();
            appuser.RefreshTokens.Add(response);
            await _forumDbContext.SaveChangesAsync();
            return Json(new {
                AccessToken = _tokenGenerator.StandardAccessToken(appuser, await roles),
                RefreshToken = response.Token
            });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(AppUserLoginPost user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var appUser = _forumDbContext.Users.Include(x => x.RefreshTokens)
                .FirstOrDefault(x => x.Email == user.Email);
            if (appUser != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(appUser, user.Password, false, false);
                if (signInResult.Succeeded) {
                    var response = _tokenGenerator.StandardRefreshToken();
                    appUser.RefreshTokens.Add(response);
                    var roles = await _userManager.GetRolesAsync(appUser);
                    await _forumDbContext.SaveChangesAsync();
                    return Json(new
                    {
                        AccessToken = _tokenGenerator.StandardAccessToken(appUser, roles),
                        RefreshToken = response.Token
                    });
                   
                }
                return BadRequest(Json(new { Error = "Bad login or password", Lockedout = signInResult.IsLockedOut}));
                
            }
            return BadRequest(Json(new { Error = "User not found", Lockedout = false }));
            

        }

        [HttpPost]
        public async Task<IActionResult> RefreshTokenAsync(string access, string refresh)
        {

            JwtSecurityToken accessToken;
            var handler = new JwtSecurityTokenHandler();
            try
            {
                accessToken = handler.ReadJwtToken(access);
            }
            catch
            {
                return BadRequest("Bad Token");
            }
            var id = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            var user = _forumDbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return BadRequest("Not Longer a member");
            _forumDbContext.RefreshTokens.RemoveRange(_forumDbContext.RefreshTokens.Where(x => x.ExpirationDate < DateTime.Now));
            var refreshToken = _forumDbContext.RefreshTokens.FirstOrDefault(x => x.Token == refresh);
            if (refreshToken == null)
            {
                await _forumDbContext.SaveChangesAsync();
                return BadRequest("Refresh Token Expired");
            }
            _forumDbContext.RefreshTokens.Remove(refreshToken);
            var response = _tokenGenerator.StandardRefreshToken();
            user.RefreshTokens.Add(response);
            var roles = await _userManager.GetRolesAsync(user);
            await _forumDbContext.SaveChangesAsync();
            return Json(new
            {
                AccessToken = _tokenGenerator.StandardAccessToken(user,  roles),
                RefreshToken = response.Token
            });

        }
    }
}