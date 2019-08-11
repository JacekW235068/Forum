using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class SubForum
    {
        [Key]
        public Guid SubForumID { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Name { get; set; }

        //Relations
        public List<ForumThread> Threads { get; set; }
    }
}
