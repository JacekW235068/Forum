using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class ForumThread
    {
        [Key]
        public Guid ThreadID { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(10)]
        public string Title { get; set; }
        [Required]
        [MaxLength(500)]
        [MinLength(30)]
        public string Text { get; set; }
        [Required]
        public DateTime PostTime { get; set; }
        [Required]
        public DateTime LastPostTime { get; set; }

        //Relations
        public AppUser User { get; set; }
        public List<Post> Comments { get; set; }
        public Guid ParentID { get; set; }
        [Required]
        [ForeignKey("ParentID")]
        public SubForum ParentForum { get; set; }
    }
}
