using Forum.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.DTO
{
    public class ForumThreadGet
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime PostTime { get; set; }
        public DateTime LastPostTime { get; set; }
        public string UserName { get; set; }
        public int Comments { get; set; }
        public string ParentForumID { get; set; }

        public ForumThreadGet() { }
        public ForumThreadGet(ForumThread thread)
        {
            ID = thread.ThreadID.ToString();
            Title = thread.Title;
            Text = thread.Text;
            PostTime = thread.PostTime;
            LastPostTime = thread.LastPostTime;
            if (thread.User != null)
                UserName = thread.User.UserName;
            Comments = thread.NumberOfComments;
            ParentForumID = thread.ParentID.ToString();
        }

        public static implicit operator ForumThreadGet(ForumThread thread)
        {
            return new ForumThreadGet(thread);
        }
    }
}
