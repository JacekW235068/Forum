using Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.DTO
{
    public class SubForumGet
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int ThreadCount { get; set; }

        public SubForumGet() { }
        public SubForumGet(SubForum subForum)
        {
            ID = subForum.SubForumID.ToString();
            Name = subForum.Name;
            ThreadCount = subForum.Threads.Count;
        }
        public static implicit operator SubForumGet(SubForum sub) {
            return new SubForumGet(sub);
        }
    }
}
