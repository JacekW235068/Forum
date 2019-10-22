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
            return JsonFormatter.SuccessResponse( 
               _databaseCache.SubForums
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> NewForumAsync([FromForm]SubForumPost Forum)
        {
            if (await _forumDBContext.SubForums.FirstOrDefaultAsync(x => x.Name == Forum.Name) != null)
                return StatusCode(400,JsonFormatter.ErrorResponse("Name is not unique" ));
            var newForum = (SubForum)Forum;
            _forumDBContext.SubForums.Add(newForum);
            await _forumDBContext.SaveChangesAsync();
            _databaseCache.RefreshSubForums(_forumDBContext);
            newForum.Threads = new List<ForumThread>();
            return Ok(
                JsonFormatter.SuccessResponse((SubForumGet)newForum
                ));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteForumAsync(string ID)
        {
            
            if (!Guid.TryParse(ID, out Guid guid))
                return StatusCode(400,JsonFormatter.FailResponse("Wrong Format"));
            if(await _forumDBContext.SubForums.FirstOrDefaultAsync(x=>x.SubForumID ==  guid) == null)
                return StatusCode(400, JsonFormatter.ErrorResponse("ID does not exist in Database"));
            _forumDBContext.SubForums.Remove(new SubForum() { SubForumID = guid });
            await _forumDBContext.SaveChangesAsync();
            _databaseCache.RefreshSubForums(_forumDBContext);
            return Ok(JsonFormatter.SuccessResponse(null));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditForumAsync(SubForumEditPost Forum)
        {
            var sub = await _forumDBContext.SubForums.FirstOrDefaultAsync(x => x.SubForumID.ToString() == Forum.SubForumID);
            if (sub == null)
                return StatusCode(400, JsonFormatter.ErrorResponse("Name is not unique"));
            sub.Name = Forum.Name;
            await _forumDBContext.SaveChangesAsync();
            _databaseCache.RefreshSubForums(_forumDBContext);
            return Ok(
                JsonFormatter.SuccessResponse((SubForumGet)sub
                ));
        }
    }
}