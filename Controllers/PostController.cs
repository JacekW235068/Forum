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

namespace Forum.Controllers { 

[Route("api/[controller]")]
[ApiController]
    public class PostController : Controller
    {
        private readonly ForumDbContext _forumDbContext;
        private readonly IDatabaseCache _databaseCache;

        public PostController(ForumDbContext forumDbContext, IDatabaseCache databaseCache)
        {
            _forumDbContext = forumDbContext;
            _databaseCache = databaseCache;
        }


        [HttpGet]
        [Route("Posts")]
        public IActionResult GetPosts(string threadID, uint start,uint amount)
        { 
            if (!Guid.TryParse(threadID, out Guid guid))
                return StatusCode(400,JsonFormatter.FailResponse("Wrong Format"));
            if (amount == 0) return StatusCode(400,JsonFormatter.FailResponse("Invalid argument"));
            var postDTO = new List<ForumPostGet>();
            var posts = _forumDbContext.Posts.Where(x => x.ParentID == guid).OrderByDescending(x=>x.PostTime).Skip((int)start).Take((int)amount).Include(x=>x.User);
            foreach (var x in posts)
                postDTO.Add(x);
            if (postDTO.Count == 0)
                return StatusCode(204, JsonFormatter.SuccessResponse(null));
            return JsonFormatter.SuccessResponse(postDTO);
        }

        [Authorize]
        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> NewPostAsync([FromForm]ForumPost_Post post)
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
            catch { return StatusCode(400,JsonFormatter.FailResponse("Bad Token")); }
            if (!Guid.TryParse(post.ParentID, out _))
                return StatusCode(400,JsonFormatter.FailResponse("Wrong Format"));
            var newPost = (Post)post;
            newPost.User = await _forumDbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            newPost.ParentThread = await _forumDbContext.Threads.FirstOrDefaultAsync(x => x.ThreadID == newPost.ParentID);
            newPost.PostTime = DateTime.Now;

            if (newPost.ParentThread == null) return NotFound(JsonFormatter.ErrorResponse("Thread Not Found"));
            if (newPost.User == null) return Unauthorized(JsonFormatter.ErrorResponse("User Not Found"));
            newPost.ParentThread.LastPostTime = DateTime.Now;
            newPost.ParentThread.NumberOfComments++;
            _forumDbContext.Posts.Add(newPost);
            await _forumDbContext.SaveChangesAsync();
            _databaseCache.UpdateThread(newPost.ParentID.ToString(), 1);
            return JsonFormatter.SuccessResponse((ForumPostGet)newPost);
        }

        [Authorize]
        [HttpDelete("{postID}")]
        [Route("Delete/{postID}")]
        public async Task<IActionResult> DeletePostAsync(string postID)
        {
            //get user info from access token
            JwtSecurityToken accessToken;
            var handler = new JwtSecurityTokenHandler();
            IEnumerable<string> role;
            string id;
            try
            {
                var trimmedToken = Request.Headers["Authorization"].ToString();
                trimmedToken = trimmedToken.Substring(7);
                accessToken = handler.ReadJwtToken(trimmedToken);
                role = accessToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x=>x.Value);
                id = accessToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            }
            catch { return StatusCode(400,JsonFormatter.FailResponse("Bad Token")); }
            var post = await _forumDbContext.Posts.Include(x => x.User).Include(x=>x.ParentThread).FirstOrDefaultAsync(x => x.PostID.ToString() == postID);
            if (post == null) return NotFound(JsonFormatter.ErrorResponse("Post does not longer exist"));
            if (role.Contains("Admin")) {
                _forumDbContext.Posts.Remove(post);
                post.ParentThread.NumberOfComments--;

                await _forumDbContext.SaveChangesAsync();
                _databaseCache.UpdateThread(post.ParentID.ToString(), -1);
                return NoContent();
            }
            if (role.Contains("NormalUser")) {
                if (post.User.Id != id) return StatusCode(403, JsonFormatter.FailResponse("Forbidden"));
                post.ParentThread.NumberOfComments--;
                _forumDbContext.Posts.Remove(post);
                await _forumDbContext.SaveChangesAsync();
                _databaseCache.UpdateThread(post.ParentID.ToString(), -1);
                return NoContent();
            }
                return StatusCode(401, JsonFormatter.FailResponse("Unauthorized"));

        }

        [Authorize]
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditPost(ForumPostEditPost post)
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
            if (!Guid.TryParse(post.PostID, out Guid postGuid))
                return StatusCode(400, JsonFormatter.FailResponse("Wrong Format"));
            var editPost = await _forumDbContext.Posts.Include(x => x.User).FirstOrDefaultAsync(x => x.PostID == postGuid);
            if (editPost == null)
            {
                return StatusCode(400, JsonFormatter.FailResponse("Thread does not exist"));
            }
            if (id != editPost.User.Id)
            {
                return StatusCode(403, JsonFormatter.FailResponse("Forbidden"));
            }
            editPost.Text = post.Text;
            await _forumDbContext.SaveChangesAsync();
            return JsonFormatter.SuccessResponse((ForumPostGet)editPost);
        }
    }
}