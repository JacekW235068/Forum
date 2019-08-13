using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers { 

[Route("api/[controller]")]
[ApiController]
public class PostController : Controller
{
    [HttpGet]
    public IActionResult GetPosts()
    {
        return Ok();
    }

    [Authorize]
    [HttpPost]
    public IActionResult NewPost()
    {
        return Ok();
    }

    [Authorize]
    [HttpDelete]
    public IActionResult DeletePost()
    {
        return Ok();
    }


}
}