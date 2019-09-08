using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        [Route("Threads")]
        public IActionResult GetThreads(string subForumID, uint start, uint amount)
        {
            Guid guid;
            if (!Guid.TryParse(subForumID, out guid))
                return BadRequest("Wrong Format");
            if (amount == 0) return BadRequest("Invalid argument");
            List<ForumThreadGet> threads = new List<ForumThreadGet>();
            foreach (var x in _forumDbContext.Threads.Where(x => x.ParentID == guid)
                .Include(x=>x.User).Skip((int)start).Take((int)amount))
                threads.Add(x);
            return Json(new
            {
                threads
            });
        }

        [HttpGet]
        [Route("Recent")]
        public IActionResult GetRecentThreads([FromQuery]uint start, [FromQuery] uint amount)
        {
            if (amount == 0) return BadRequest("Invalid argument");
            List<ForumThreadGet> threads = new List<ForumThreadGet>();
            threads.AddRange(_databaseCache.GetThreads(_forumDbContext, start, amount));
            return Json(new
            {
                threads
            });
        }

        [Authorize]
        [HttpGet]
        [Route("UserThreads")]
        public IActionResult GetUserThreads([FromForm]string userName, [FromForm]uint start, [FromForm]uint amount)
        {
            if (amount == 0) return BadRequest("Invalid argument");
            var user = _forumDbContext.Users.Where(x => x.UserName == userName).FirstOrDefault();
            if (user == null) return NotFound("User No longer Exists");
            var threads = new List<ForumThreadGet>();
            foreach (var x in _forumDbContext.Threads.Where(x => x.User == user)
                .Skip((int)start).Take((int)amount)
                .Include(x => x.User))
                threads.Add(x);
            return Json(threads);
        }

        [Authorize]
        [HttpPost]
        [Route("New")]
        public IActionResult NewThread([FromForm]ForumThreadPost thread)
        {
            //get user info from access token
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
            catch { return BadRequest("Bad Token"); }
            if (!Guid.TryParse(thread.SubForumID, out _))
                return BadRequest("Wrong Format");
            var newThread = (ForumThread)thread;
            newThread.User = _forumDbContext.Users.FirstOrDefault(x => x.Id == id);
            newThread.ParentForum = _forumDbContext.SubForums.FirstOrDefault(x => x.SubForumID == newThread.ParentID);
            if (newThread.User == null) return Unauthorized();
            if (newThread.ParentForum == null) return NotFound("Forum Not Found");
            newThread.LastPostTime = DateTime.Now;
            newThread.PostTime = DateTime.Now;
            _forumDbContext.Threads.Add(newThread);
            _forumDbContext.SaveChanges();
            newThread.Comments = new List<Post>();
            _databaseCache.AddThread(newThread);
            return Ok(Json(newThread.ThreadID));
        }

        [Authorize]
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteThreadAsync([FromForm]string threadID)
        {
            JwtSecurityToken accessToken;
            var handler = new JwtSecurityTokenHandler();
            IEnumerable<string> roles;
            string id;
            try
            {
                var trimmedToken = Request.Headers["Authorization"].ToString();
                trimmedToken = trimmedToken.Substring(7);
                accessToken = handler.ReadJwtToken(trimmedToken);
                roles = accessToken.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(x=>x.Value);
                id = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            catch { return BadRequest("Bad Token"); }
            Guid guid;
            if (!Guid.TryParse(threadID, out guid))
                return BadRequest("Wrong Format");
            var thread = await _forumDbContext.Threads.Include(x=> x.User).FirstOrDefaultAsync(x => x.ThreadID == guid);
            if (thread == null) NotFound("Thread does no longer exist");
            if (roles.Contains("Admin"))
            {
                _databaseCache.DeleteThread(thread.ThreadID.ToString());
                _forumDbContext.Threads.Remove(thread);
                _forumDbContext.SaveChanges();
                return Ok();
            }
            if (roles.Contains("NormalUser"))
            {
                if (thread.User.Id != accessToken.Claims.FirstOrDefault(y => y.Type == ClaimTypes.NameIdentifier).Value) return Unauthorized();
                _forumDbContext.Threads.Remove(thread);
                _forumDbContext.SaveChanges();
                return Ok();
            }
            return Unauthorized();
                    
                   
                        
                    
                
            
        }


    }
}
