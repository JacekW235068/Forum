using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models;
using Forum.Services;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
            var appuser = new AppUser()
            {
                Email = user.Email,
                UserName = user.Username,
            };
            var createResult = await _userManager.CreateAsync(appuser, user.Password);
            if (!createResult.Succeeded)
            {
                return StatusCode(400,JsonFormatter.ErrorResponse("Problem Creating A User", createResult.Errors));
            }
            await _userManager.AddToRoleAsync(appuser, "NormalUser");
            var roles = await _userManager.GetRolesAsync(appuser);
            appuser = _forumDbContext.Users.Include(x => x.RefreshTokens)
                .FirstOrDefault(x => x.Email == appuser.Email);
            var response = _tokenGenerator.StandardRefreshToken();
            appuser.RefreshTokens.Add(response);
            await _forumDbContext.SaveChangesAsync();
            await _signInManager.SignInAsync(appuser, false);
            return JsonFormatter.SuccessResponse(new
            {
                AccessToken = _tokenGenerator.StandardAccessToken(appuser, roles),
                RefreshToken = response.Token
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync([FromForm]AppUserLoginPost user)
        {
            Response.ContentType = "Application/json";
            var appUser = _forumDbContext.Users.Include(x => x.RefreshTokens)
                .FirstOrDefault(x => x.Email == user.Email);
            if (appUser == null)
            { return StatusCode(400,JsonFormatter.ErrorResponse("User Not Found")); }
            var signInResult = await _signInManager.PasswordSignInAsync(appUser, user.Password, false, true);
            if (!signInResult.Succeeded) return StatusCode(400,JsonFormatter.ErrorResponse("Login failed", new { code = "Lockedout", Description = signInResult.IsLockedOut }));
            var response = _tokenGenerator.StandardRefreshToken();
            appUser.RefreshTokens.Add(response);
            var roles = await _userManager.GetRolesAsync(appUser);
            await _forumDbContext.SaveChangesAsync();
            return JsonFormatter.SuccessResponse(new
            {
                AccessToken = _tokenGenerator.StandardAccessToken(appUser, roles),
                RefreshToken = response.Token
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync([FromForm]string access, [FromForm]string refresh)
        {
            string id;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken accessToken;
                accessToken = handler.ReadJwtToken(access);
                id = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            catch
            {
                return StatusCode(400,JsonFormatter.ErrorResponse("Bad Token"));
            }

            var user = _forumDbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return StatusCode(400,JsonFormatter.ErrorResponse("User does not exist"));
            _forumDbContext.RefreshTokens.RemoveRange(_forumDbContext.RefreshTokens.Where(x => x.ExpirationDate < DateTime.Now));
            var refreshToken = _forumDbContext.RefreshTokens.FirstOrDefault(x => x.Token == refresh);
            if (refreshToken == null)
            {
                await _forumDbContext.SaveChangesAsync();
                return StatusCode(400,JsonFormatter.ErrorResponse("Token Expired"));
            }
            _forumDbContext.RefreshTokens.Remove(refreshToken);
            var response = _tokenGenerator.StandardRefreshToken();
            user.RefreshTokens.Add(response);
            var roles = await _userManager.GetRolesAsync(user);
            await _forumDbContext.SaveChangesAsync();
            return JsonFormatter.SuccessResponse(new
            {
                AccessToken = _tokenGenerator.StandardAccessToken(user, roles),
                RefreshToken = response.Token
            });
        }


        [HttpPost]
        [Authorize]
        [Route("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            JwtSecurityToken accessToken;
            var handler = new JwtSecurityTokenHandler();
            string id;
            try
            {
                var trimmedToken = Request.Headers["Authorization"].ToString();
                trimmedToken = trimmedToken.Substring(7);
                accessToken = handler.ReadJwtToken(trimmedToken);
                id = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            catch
            {
                return StatusCode(400, JsonFormatter.ErrorResponse("Bad Token"));
            }

            var user = _forumDbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return StatusCode(400, JsonFormatter.ErrorResponse("User does not exist"));
            _forumDbContext.RefreshTokens.RemoveRange(_forumDbContext.RefreshTokens.Where(x => x.user == user));
            await _signInManager.SignOutAsync();
            await _forumDbContext.SaveChangesAsync();
            return JsonFormatter.SuccessResponse(null);
        }
    }
}