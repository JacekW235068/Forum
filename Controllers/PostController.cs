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
        public IActionResult GetPosts(string threadID, int start, int amount)
        {
            var postViewModels = new List<ForumPostGet>();
            var posts = _forumDbContext.Posts.Where(x => x.ParentID.ToString() == threadID).Skip(start).Take(amount);
            if (posts == null) return NotFound();
            foreach (var x in posts)
                postViewModels.Add(x);
            return Json(new
            {
                postViewModels
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewPost(ForumPost_Post post)
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

            var newPost = (Post)post;
            newPost.User = _forumDbContext.Users.FirstOrDefault(x => x.Id == id);
            newPost.ParentThread = _forumDbContext.Threads.FirstOrDefault(x => x.ThreadID == newPost.ParentID);
            newPost.PostTime = DateTime.Now;
            if (newPost.ParentThread == null) return NotFound("thread does not exist");
            if (newPost.User == null) return Unauthorized();
            _forumDbContext.Posts.Add(newPost);
            _forumDbContext.SaveChanges();
            _databaseCache.AddThread(newPost.ParentThread);
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeletePost(string postID)
        {
            //get user info from access token
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
            var post = _forumDbContext.Posts.Include(x => x.User).FirstOrDefault(x => x.PostID.ToString() == postID);
            switch (role)
            {
                case "Admin":
                    if (post != null)
                    {
                        _forumDbContext.Posts.Remove(post);
                        _forumDbContext.SaveChanges();
                        return Ok();
                    }
                    return NotFound("Post does no longer exist");
                case "NormalUser":

                    if (post != null)
                    {
                        if (post.User.Id != id) return Unauthorized();
                        _forumDbContext.Posts.Remove(post);
                        _forumDbContext.SaveChanges();
                        return Ok();
                    }
                    return NotFound("Post does no longer exist");
                default:
                    return Unauthorized();
            }
        }


    }
}