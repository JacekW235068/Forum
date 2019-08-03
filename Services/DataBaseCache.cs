using Forum.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    public class DataBaseCache
    {
        private  List<ForumThread> threads;
        public IReadOnlyCollection<ForumThread> Threads { get { return threads.AsReadOnly(); } }
        public DataBaseCache()
        {

        }
        public void Init(ForumDbContext dbContext)
        {
            threads = dbContext.Threads.OrderByDescending(x => x.LastPostTime).Take(30).ToList();
        }
        public void Add(ForumThread thread)
        {
            if (thread == null ||
                thread.ThreadID == null ||
                thread.Title == null ||
                thread.Text == null)
                throw new NullReferenceException();
            var i = threads.Select(x => x.ThreadID).ToList().IndexOf(thread.ThreadID);
            if (i == -1)
                threads.RemoveAt(29);
            else
                threads.RemoveAt(i);
            threads.Add(thread);
        }
    }
}
