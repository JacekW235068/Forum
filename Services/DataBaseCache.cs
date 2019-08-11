using Forum.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    public class DataBaseCache : IDatabaseCache
    {
        private  List<ForumThread> threads;
        private int MaxItems;
        public IReadOnlyCollection<ForumThread> Threads { get { return threads.AsReadOnly(); } }
       
        public DataBaseCache(int maxItems)
        {
            MaxItems = maxItems;
        }
              
        public void Init(ForumDbContext context)
        {
            if (threads == null)
                threads = context.Threads.OrderByDescending(x => x.LastPostTime).Take(MaxItems).ToList();
        }
        public void Add(ForumThread thread)
        {
            if (thread == null ||
                thread.ThreadID == null ||
                thread.Title == null ||
                thread.Text == null)
                throw new NullReferenceException();

            var i = threads.Select(x => x.ThreadID).ToList().IndexOf(thread.ThreadID);
            if(threads.Count == MaxItems)
                if (i == -1)
                    threads.RemoveAt(MaxItems - 1);
                else
                    threads.RemoveAt(i);
            threads.Add(thread);
        }
    }
}
