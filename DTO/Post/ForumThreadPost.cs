using Forum.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.DTO
{
    public class ForumThreadPost
    {
        [Required]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Title has to be between 5 and 50 characters long")]
        public string Title { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 30, ErrorMessage = "Message has to be between 30 and 500 characters long")]
        public string Text { get; set; }
        [Required]
        public string SubForumID { get; set; }

        public static explicit operator ForumThread(ForumThreadPost thread)
        {
            return new ForumThread() {
                Title = thread.Title,
                Text = thread.Text,
                ParentID = Guid.Parse(thread.SubForumID)
            };
        }
    }
}
