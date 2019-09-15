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
                return BadRequest(JsonFormatter.FailResponse("Wrong Format"));
            if (amount == 0) return BadRequest(JsonFormatter.FailResponse("Invalid argument"));
            var postViewModels = new List<ForumPostGet>();
            var posts = _forumDbContext.Posts.Where(x => x.ParentID == guid).OrderBy(x=>x.PostTime).Skip((int)start).Take((int)amount).Include(x=>x.User);
            foreach (var x in posts)
                postViewModels.Add(x);
            if (postViewModels.Count == 0)
                return StatusCode(204, JsonFormatter.SuccessResponse(null));
            return JsonFormatter.SuccessResponse(postViewModels);
        }

        [Authorize]
        [HttpPost]
        [Route("New")]
        public IActionResult NewPost([FromForm]ForumPost_Post post)
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
            catch { return BadRequest(JsonFormatter.FailResponse("Bad Token")); }
            if (!Guid.TryParse(post.ParentID, out _))
                return BadRequest(JsonFormatter.FailResponse("Wrong Format"));
            var newPost = (Post)post;
            newPost.User = _forumDbContext.Users.FirstOrDefault(x => x.Id == id);
            newPost.ParentThread = _forumDbContext.Threads.FirstOrDefault(x => x.ThreadID == newPost.ParentID);
            newPost.PostTime = DateTime.Now;

            if (newPost.ParentThread == null) return NotFound(JsonFormatter.ErrorResponse("Thread Not Found"));
            if (newPost.User == null) return Unauthorized(JsonFormatter.ErrorResponse("User Not Found"));
            newPost.ParentThread.LastPostTime = DateTime.Now;
            newPost.ParentThread.NumberOfComments++;
            _forumDbContext.Posts.Add(newPost);
            _forumDbContext.SaveChanges();
            _databaseCache.UpdateThread(newPost.ParentID.ToString(), 1);
            return Ok(JsonFormatter.SuccessResponse((ForumPostGet)newPost));
        }

        [Authorize]
        [HttpDelete]
        [Route("Delete")]
        public IActionResult DeletePost([FromForm]string postID)
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
            catch { return BadRequest(JsonFormatter.FailResponse("Bad Token")); }
            var post = _forumDbContext.Posts.Include(x => x.User).Include(x=>x.ParentThread).FirstOrDefault(x => x.PostID.ToString() == postID);
            if (post == null) return NotFound(JsonFormatter.ErrorResponse("Post does not longer exist"));
            if (role.Contains("Admin")) {
                _forumDbContext.Posts.Remove(post);
                post.ParentThread.NumberOfComments--;

                _forumDbContext.SaveChanges();
                _databaseCache.UpdateThread(post.ParentID.ToString(), -1);
                return Ok(JsonFormatter.SuccessResponse(null));
            }
            if (role.Contains("NormalUser")) {
                if (post.User.Id != id) return StatusCode(403, JsonFormatter.FailResponse("Forbidden"));
                post.ParentThread.NumberOfComments--;
                _forumDbContext.Posts.Remove(post);
                _forumDbContext.SaveChanges();
                _databaseCache.UpdateThread(post.ParentID.ToString(), -1);
                return Ok(JsonFormatter.SuccessResponse(null));
            }
                return StatusCode(403, JsonFormatter.FailResponse("Forbidden"));

        }


    }
}