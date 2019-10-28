using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Models;
using Forum.Services;
using Forum.DTO;
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
        [Route("Thread")]
        public IActionResult GetThread(string threadID)
        {
            if (!Guid.TryParse(threadID, out Guid guid))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            ForumThreadGet thread = _forumDbContext.Threads.Include(x => x.User).FirstOrDefault(x => x.ThreadID == guid);
            if (thread == null)
                return StatusCode(204, JsonFormatter.SuccessResponse(null));
            return JsonFormatter.SuccessResponse(thread);
        }


        [HttpGet]
        [Route("Threads")]
        public IActionResult GetThreads(string subForumID, uint start, uint amount)
        {
            if (!Guid.TryParse(subForumID, out Guid guid))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            if (amount == 0) return StatusCode(400, JsonFormatter.FailResponse("Invalid argument"));
            List<ForumThreadGet> threads = new List<ForumThreadGet>();
            foreach (var x in _forumDbContext.Threads.Where(x => x.ParentID == guid)
                .Include(x => x.User).OrderBy(x => x.PostTime).Skip((int)start).Take((int)amount))
                threads.Add(x);
            if (threads.Count == 0)
                return StatusCode(204, JsonFormatter.SuccessResponse(null));
            return JsonFormatter.SuccessResponse(threads);
        }

        [HttpGet]
        [Route("Recent")]
        public IActionResult GetRecentThreads([FromQuery]uint start, [FromQuery] uint amount)
        {
            if (amount == 0) return StatusCode(400, JsonFormatter.FailResponse("Invalid argument"));
            List<ForumThreadGet> threads = new List<ForumThreadGet>();
            threads.AddRange(_databaseCache.GetThreads(_forumDbContext, start, amount));
            if (threads.Count == 0)
                return StatusCode(204, JsonFormatter.SuccessResponse(null));
            return JsonFormatter.SuccessResponse(threads);
        }

        [Authorize]
        [HttpGet]
        [Route("UserThreads")]
        public IActionResult GetUserThreads([FromForm]string userName, [FromForm]uint start, [FromForm]uint amount)
        {
            if (amount == 0) return StatusCode(400, "Invalid argument");
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
            catch { return StatusCode(400, JsonFormatter.FailResponse("Bad Token")); }
            if (!Guid.TryParse(thread.SubForumID, out _))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            var newThread = (ForumThread)thread;
            newThread.User = _forumDbContext.Users.FirstOrDefault(x => x.Id == id);
            newThread.ParentForum = _forumDbContext.SubForums.FirstOrDefault(x => x.SubForumID == newThread.ParentID);
            if (newThread.User == null) return Unauthorized(JsonFormatter.ErrorResponse("User Not Found"));
            if (newThread.ParentForum == null) return NotFound(JsonFormatter.ErrorResponse("Forum Not Found"));
            newThread.LastPostTime = DateTime.Now;
            newThread.PostTime = DateTime.Now;
            _forumDbContext.Threads.Add(newThread);
            _forumDbContext.SaveChanges();
            newThread.Comments = new List<Post>();
            _databaseCache.AddThread(newThread);
            return JsonFormatter.SuccessResponse((ForumThreadGet)newThread);
        }

        [Authorize]
        [HttpDelete("{id}")]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteThreadAsync(string ID)
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
                roles = accessToken.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(x => x.Value);
                id = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            catch { return StatusCode(400, JsonFormatter.FailResponse("Bad Token")); }
            if (!Guid.TryParse(ID, out Guid guid))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            var thread = await _forumDbContext.Threads.Include(x => x.User).FirstOrDefaultAsync(x => x.ThreadID == guid);
            if (thread == null) NotFound("Thread does no longer exist");
            if (roles.Contains("Admin"))
            {
                _databaseCache.DeleteThread(thread);
                _forumDbContext.Threads.Remove(thread);
                _forumDbContext.SaveChanges();
                return NoContent();
            }
            if (roles.Contains("NormalUser"))
            {
                if (thread.User.Id != accessToken.Claims.FirstOrDefault(y => y.Type == ClaimTypes.NameIdentifier).Value)
                    return StatusCode(403, JsonFormatter.FailResponse("Forbidden"));
                _forumDbContext.Threads.Remove(thread);
                _forumDbContext.SaveChanges();
                return NoContent();
            }
            return StatusCode(403, JsonFormatter.FailResponse("Forbidden"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Move")]
        public async Task<IActionResult> MoveThread(MoveThreadPost IDs)
        {
      
            if (!Guid.TryParse(IDs.threadID, out Guid threadGuid))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            if (!Guid.TryParse(IDs.subForumID, out Guid subGuid))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            var thread = await _forumDbContext.Threads.FirstOrDefaultAsync(x => x.ThreadID == threadGuid);
            var sub = await _forumDbContext.SubForums.FirstOrDefaultAsync(x => x.SubForumID == subGuid);
            if (thread == null)
            {
                return StatusCode(400, JsonFormatter.FailResponse("Thread does not exist"));
            }
            if (sub == null)
            {
                return StatusCode(400, JsonFormatter.FailResponse("Forum destination does not exist"));
            }
            _databaseCache.MoveThread(thread.ParentID.ToString(), sub.SubForumID.ToString() );
            thread.ParentForum = sub;
            await _forumDbContext.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditThread([FromForm]ForumThreadEditPost thread)
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
                roles = accessToken.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(x => x.Value);
                id = accessToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            catch { return StatusCode(400, JsonFormatter.FailResponse("Bad Token")); }
            if (!Guid.TryParse(thread.ThreadID, out Guid threadGuid))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            var editThread = await _forumDbContext.Threads.Include(x=>x.User).FirstOrDefaultAsync(x => x.ThreadID == threadGuid);
            if (editThread == null)
            {
                return StatusCode(400, JsonFormatter.FailResponse("Thread does not exist"));
            }
            editThread.Title = thread.Title;
            editThread.Text = thread.Text;
            await _forumDbContext.SaveChangesAsync();
            return JsonFormatter.SuccessResponse((ForumThreadGet)editThread);
        }
    }
}
