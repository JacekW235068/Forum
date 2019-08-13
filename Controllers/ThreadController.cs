using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreadController : Controller
    {
        [HttpGet]
        public IActionResult GetThreads()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult GetRecentThreads()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUserThreads()
        {
            return Ok();
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewThread()
        {
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteThread()
        {
            return Ok();
        }


    }
}
