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
    //VERY TEMPLATE ACCOUNT CONTROLLER
    //TODO: update
    public class AccountController : Controller
    {
        private readonly ForumDbContext _forumDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenFactory _tokenGenerator;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(ForumDbContext context,
            UserManager<AppUser> userManger,
            ITokenFactory access,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManger;
            _forumDbContext = context;
            _tokenGenerator = access;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //TODO: Check userManager, give proper status codes, Mapping
        // [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(AppUserRegisterPost user)
        {
            if (!ModelState.IsValid) return BadRequest();
            var appuser = new AppUser()
            {
                Email = user.Email,
                UserName = user.Username,
            };

            if (!(await _userManager.CreateAsync(appuser, user.Password)).Succeeded) {
                return BadRequest();
            }
            await _userManager.AddToRoleAsync(appuser, "NormalUser");
            var roles = _userManager.GetRolesAsync(appuser);
            await _signInManager.SignInAsync(appuser, false);
            appuser = _forumDbContext.Users.Include(x => x.RefreshTokens).FirstOrDefault(x => x.Email == appuser.Email && x.UserName == appuser.UserName);
            var response = _tokenGenerator.StandardRefreshToken();
            appuser.RefreshTokens.Add(response);
         
           
            //TODO: add roles
            return Json(new {
                AccessToken = _tokenGenerator.StandardAccessToken(appuser, await roles),
                RefreshToken = response
            });
        }

        //TODO: Same as Register
        public async Task<IActionResult> LoginAsync(AppUserLoginPost user)
        {
            if (!ModelState.IsValid) return BadRequest();
            var appUser = _forumDbContext.Users.FirstOrDefault(x => x.Email == user.Email);
            if (appUser != null)
            {
                if ((await _signInManager.PasswordSignInAsync(appUser,user.Password,false,false)).Succeeded) {
                    var response = _tokenGenerator.StandardRefreshToken();
                    appUser.RefreshTokens.Add(response);
                    var roles = _userManager.GetRolesAsync(appUser);
                    return Json(new
                    {
                        AccessToken = _tokenGenerator.StandardAccessToken(appUser, await roles),
                        RefreshToken = response
                    });
                }
                
            }
            return BadRequest();
            

        }

    }
}