using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models;

namespace Forum.DTO
{
    public class ForumPost_Post
    {
        [Required]
        [StringLength(500, MinimumLength = 30, ErrorMessage = "Text has to be between 30 and 500 characters long")]
        public string Text { get; set; }
        [Required]
        public string ParentID { get; set; }
        public static explicit operator Post(ForumPost_Post post)
        {
            return new Post()
            {
                Text = post.Text,
                ParentID = Guid.Parse(post.ParentID)
            };
        }
    }
}
