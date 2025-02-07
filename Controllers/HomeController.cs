﻿using System;
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
using Forum.Services;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ForumDbContext _forumDbContext;
        private readonly IDatabaseCache _databaseCache;
        public HomeController(ForumDbContext forumDBContext, IDatabaseCache databaseCache)
        {
            _forumDbContext = forumDBContext;
            _databaseCache = databaseCache;
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

        [Route("Threads")]
        public async Task<IActionResult> ThreadsAsync(string subID)
        {

            if (subID == null)
                return BadRequest();
            if (!Guid.TryParse(subID, out Guid guid))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            var Title = (await _forumDbContext.SubForums.FirstOrDefaultAsync(x => x.SubForumID == guid)).Name;
            ViewData["Title"] = Title;
            return View();
        }

        [Route("Subs")]
        public IActionResult SubForumNavigation()
        {
            return View(_databaseCache.SubForums);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
           
            HttpContext.Response.StatusCode = 500;
            //scetchy af
            return JsonFormatter.ErrorResponse("Fatal Error",new List<object>());
        }

        //just for tests
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

    
    }
}
