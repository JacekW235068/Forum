using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class Post
    {
        [Key]
        public Guid PostID { get; set; }
        [Required]
        [MaxLength(500)]
        [MinLength(30)]
        public string Text { get; set; }
        [Required]
        public DateTime PostTime { get; set; }

        //Relations
        public AppUser User { get; set; }
        [Required]
        public ForumThread ParentThread { get; set; }
    }
}
