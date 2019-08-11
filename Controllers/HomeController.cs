using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ForumDbContext DbContext;    
        public HomeController(ForumDbContext forumDbContext)
        {
            DbContext = forumDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
