using Forum.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.DTO
{
    public class SubForumPost
    {
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Name has to be between 5 and 100 characters long")]
        public string Name { get; set; }

        public SubForumPost() { }
        
        public static explicit operator SubForum(SubForumPost sub)
        {
            return new SubForum()
            {
                Name = sub.Name
            };
        }
     
    }
}
