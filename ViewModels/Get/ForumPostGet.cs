using Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class ForumPostGet
    {
        public string PostID { get; set; }
        public string Text { get; set; }
        public DateTime PostTime { get; set; }
        public string UserName { get; set; }

        public ForumPostGet(Post post)
        {
            PostID = post.PostID.ToString();
            Text = post.Text;
            PostTime = post.PostTime;
            UserName = post.User.UserName;
        }

        public static implicit operator ForumPostGet(Post post)
        {
            return new ForumPostGet(post);
        }
    }
}
