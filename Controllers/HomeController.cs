using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ForumDbContext _forumDbContext;    
        public HomeController(ForumDbContext forumDbContext)
        {
            _forumDbContext = forumDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult RegisterForm()
        {
            return View();
        }

        public IActionResult AccountInfo(string accessToken)
        {
            if (accessToken != null)
            {
                string userID;
                try
                {
                    JwtSecurityToken AccessToken;
                    var handler = new JwtSecurityTokenHandler();
                    var trimmedToken = Request.Headers["Authorization"].ToString();
                    trimmedToken = trimmedToken.Substring(7);
                    AccessToken = handler.ReadJwtToken(trimmedToken);
                    if (AccessToken.ValidTo < DateTime.Now) return BadRequest("Token Expired");
                    userID = AccessToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                }
                catch { return BadRequest("Bad Token"); }
                var user = _forumDbContext.Users.FirstOrDefault(x => x.Id == userID).UserName;
                if (user == null) BadRequest();
                var userroles = _forumDbContext.UserRoles.Where(y => y.UserId == userID).ToArray();
                var roles = _forumDbContext.Roles.Where(x => userroles.Any(y => y.RoleId == x.Id)).Select(x => x.Name).ToArray();
                //add parameters for viewbag
                ViewBag["Name"] = user;
                ViewBag["Roles"] = roles;


            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult privateaction()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult adminaction()
        {
            return Ok();
        }
        [Authorize(Roles = "NormalUser")]
        [HttpPost]
        public IActionResult useraction()
        {
            
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            HttpContext.Response.StatusCode = 500;

            return Json(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = 500 });
        }
    }
}
