using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models;
using Forum.Services;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : Controller
    {
        
        private readonly ForumDbContext _forumDBContext;
        private readonly IDatabaseCache _databaseCache; 

        public ForumController(ForumDbContext forumDBContext, IDatabaseCache databaseCache)
        {
            _forumDBContext = forumDBContext;
            _databaseCache = databaseCache;
        }


        [HttpGet]
        public JsonResult GetAllForums()
        {          
            return Json(new {
                _databaseCache.SubForums
            });    
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult NewForum(SubForumPost newForum)
        {
            //if (!ModelState.IsValid) BadRequest(ModelState);
            _forumDBContext.SubForums.Add((SubForum)newForum);
            _forumDBContext.SaveChanges();
            _databaseCache.RefreshSubForums(_forumDBContext);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public IActionResult DeleteForum(string ID)
        {
            _forumDBContext.SubForums.Remove(new SubForum() { SubForumID = Guid.Parse(ID)});
            _forumDBContext.SaveChanges();
            _databaseCache.RefreshSubForums(_forumDBContext);
            return Ok();
        }


    }
}