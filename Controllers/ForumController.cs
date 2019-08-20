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
using Microsoft.EntityFrameworkCore;

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
        [Route("AllForums")]
        public JsonResult GetAllForums()
        {
            return Json(new {
                _databaseCache.SubForums
            });
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("New")]
        public IActionResult NewForum([FromForm]SubForumPost Forum)
        {
            if (_forumDBContext.SubForums.FirstOrDefault(x => x.Name == Forum.Name) != null) return BadRequest("Name is not unique");
            var newForum = (SubForum)Forum;
            _forumDBContext.SubForums.Add(newForum);
            _forumDBContext.SaveChanges();
            _databaseCache.RefreshSubForums(_forumDBContext);
            newForum.Threads = new List<ForumThread>();
            SubForumGet Result = newForum;
            return Ok(Json(Result));
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("Delete")]
        public IActionResult DeleteForum([FromForm]string ID)
        {
            Guid guid;
            if (!Guid.TryParse(ID, out guid))
                return BadRequest("Wrong Format");
            _forumDBContext.SubForums.Remove(new SubForum() { SubForumID = Guid.Parse(ID)});
            try
            {
                _forumDBContext.SaveChanges();
            }
            catch
            {
                return BadRequest("ID does not exist in Database");
            }
            _databaseCache.RefreshSubForums(_forumDBContext);
            return Ok();
        }


    }
}