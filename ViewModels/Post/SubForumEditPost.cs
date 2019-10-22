using Forum.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class SubForumEditPost
    {
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Name has to be between 5 and 100 characters long")]
        public string Name { get; set; }
        [Required]
        public string SubForumID { get; set; }
        public SubForumEditPost() { }

        public static explicit operator SubForum(SubForumEditPost sub)
        {
            return new SubForum()
            {
                SubForumID = Guid.Parse(sub.SubForumID),
                Name = sub.Name
            };
        }
    }
}
