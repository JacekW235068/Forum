using Forum.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class ForumPostEditPost
    {
        [Required]
        public string PostID { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 30, ErrorMessage = "Text has to be between 30 and 500 characters long")]
        public string Text { get; set; }
       
        
        public static explicit operator Post(ForumPostEditPost post)
        {
            return new Post()
            {
                PostID = Guid.Parse(post.PostID),
                Text = post.Text,
                
            };
        }
    }
}
