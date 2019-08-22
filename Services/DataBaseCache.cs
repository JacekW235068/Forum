using Forum.Models;
using Forum.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    //TODO: Add updating Thread
    public class DataBaseCache : IDatabaseCache
    {
        private readonly List<SubForumGet> subForums;
        private readonly List<ForumThreadGet> threads;
        private readonly int maxThreads;
        public IReadOnlyCollection<ForumThreadGet> Threads { get {
                return threads;
            } }
        public int MaxThreads { get { return maxThreads; } }
        public IReadOnlyCollection<SubForumGet> SubForums { get { return subForums.AsReadOnly(); } }
       
        public DataBaseCache(int maxItems)
        {
            maxThreads = maxItems;
            subForums = new List<SubForumGet>();
            threads = new List<ForumThreadGet>();
        }
              
        public void InitForumThreads(ForumDbContext context)
        {
            threads.Clear();
            foreach (var t in context.Threads.Include(x=>x.Comments).Include(x=>x.User).OrderByDescending(x => x.LastPostTime).Take(maxThreads).ToList())
                threads.Add(t);
        }
        public void AddThread(ForumThread thread)
        {
            if (thread.ThreadID == null ||
                thread.Title == null ||
                thread.Text == null)
                throw new NullReferenceException();

            
            if(threads.Count == maxThreads)               
                    threads.RemoveAt(maxThreads - 1);
            threads.Insert(0,thread);
        }
        public bool DeleteThread(string Id)
        {
            var i = threads.Select(x => x.ID).ToList().IndexOf(Id);
            if (i == -1)
                return false;
            threads.RemoveAt(i);
            return true;
        }

        public void RefreshSubForums(ForumDbContext context)
        {
            subForums.Clear();
            foreach (var sub in context.SubForums.Include(x=>x.Threads))
            {
                subForums.Add(sub);
            }
        }

        public bool UpdateThread(string threadID, int postCount)
        {
            foreach(var thread in threads) 
                if(thread.ID == threadID)
                {
                    thread.Comments += postCount;
                    if (postCount > 0)
                    {
                        thread.LastPostTime = DateTime.Now;
                        threads.Remove(thread);
                        threads.Insert(0, thread);
                        
                    }
                    return true;
                }
            return false;
        }
    }
}
