using Forum.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class ForumThreadPost
    {
        [Required]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Title most be between 5 and 100 characters")]
        public string Title { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 30, ErrorMessage = "Message most be between 30 and 500 characters")]
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
