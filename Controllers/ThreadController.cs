using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models;
using Forum.Services;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreadController : Controller
    {
        private readonly ForumDbContext _forumDbContext;
        private readonly IDatabaseCache _databaseCache;

        public ThreadController(ForumDbContext forumDbContext, IDatabaseCache databaseCache)
        {
            _forumDbContext = forumDbContext;
            _databaseCache = databaseCache;
        }

        [HttpGet]
        public JsonResult GetThreads(string subForumID, int start, int amount)
        {
            List<ForumThreadGet> threads = new List<ForumThreadGet>();
            foreach (var x in _forumDbContext.Threads.Where(x => x.ParentID.ToString() == subForumID).Skip(start).Take(amount))
                threads.Add(x);
            return Json(new
            {
                threads
            });
        }

        [HttpGet]
        public JsonResult GetRecentThreads(int start, int amount)
        {
            List<ForumThreadGet> threads = new List<ForumThreadGet>();
            //this logic should be in cache but it isn't ¯\_(ツ)_/¯
            if (start + amount >= _databaseCache.MaxThreads)
                foreach (var x in _forumDbContext.Threads.OrderByDescending(x => x.LastPostTime).Skip(start).Take(amount))
                    threads.Add(x);
            else
               threads.AddRange(_databaseCache.Threads.Skip(start).Take(amount));
            return Json(new
            {
                threads
            });
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUserThreads(string userName, int start, int amount)
        {
            var user = _forumDbContext.Users.Where(x => x.UserName == userName).FirstOrDefault();
            if (user == null) return NotFound("User No longer Exists");
            var threads = new List<ForumThreadGet>();
            foreach (var x in _forumDbContext.Threads.Where(x => x.User == user).Skip(start).Take(amount))
                threads.Add(x);
            return Json(threads);
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewThread(ForumThreadPost threadPost)
        {
            //get user info from access token
            JwtSecurityToken accessToken;
            var handler = new JwtSecurityTokenHandler();
            string id;
            try
            {
                accessToken = handler.ReadJwtToken(Request.Headers["Authorization"]);
                id = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            catch { return BadRequest("Bad Token"); }

            var thread = (ForumThread)threadPost;
            thread.User = new AppUser() { Id = id };
            thread.LastPostTime = DateTime.Now;
            thread.PostTime = DateTime.Now;
            _forumDbContext.Threads.Add(thread);
            _forumDbContext.SaveChanges();
            _databaseCache.AddThread(thread);//needs special attention
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteThreadAsync(string threadID)
        {
            JwtSecurityToken accessToken;
            var handler = new JwtSecurityTokenHandler();
            string role;
            string id;
            try
            {
                accessToken = handler.ReadJwtToken(Request.Headers["Authorization"]);
                role = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                id = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            catch { return BadRequest("Bad Token"); }
            var thread = _forumDbContext.Threads.Include(x=> x.User).Where(x => x.ThreadID == Guid.Parse(threadID)).FirstOrDefaultAsync();
            switch (role)
            {
                case "Admin":
                    if (await thread != null) {
                        _forumDbContext.Threads.Remove(new ForumThread() { ThreadID = Guid.Parse(threadID) });
                        _forumDbContext.SaveChanges();
                        return Ok();
                    }
                    return NotFound("Thread does no longer exist");
                case "NormalUser":
                    
                    if ((await thread) != null)
                    {
                        if ((await thread).User.Id != id) return Unauthorized(); 
                        _forumDbContext.Threads.Remove(new ForumThread() { ThreadID = Guid.Parse(threadID) });
                        _forumDbContext.SaveChanges();
                        return Ok();
                    }
                    return NotFound("Thread does no longer exist");
                default:
                    return Unauthorized();
            }
        }


    }
}
