using Forum.Models;
using Forum.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    //TODO: Change ForumThreads to viewmodels
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
            foreach (var t in context.Threads.OrderByDescending(x => x.LastPostTime).Take(maxThreads).ToList())
                threads.Add(t);
        }
        public void AddThread(ForumThread thread)
        {
            if (thread.ThreadID == null ||
                thread.Title == null ||
                thread.Text == null)
                throw new NullReferenceException();

            var i = threads.Select(x => x.ID).ToList().IndexOf(thread.ThreadID.ToString());
            if(threads.Count == maxThreads)
                if (i == -1)
                    threads.RemoveAt(maxThreads - 1);
                else
                    threads.RemoveAt(i);
            threads.Add(thread);
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
            foreach (var sub in context.SubForums)
            {
                subForums.Add(sub);
            }
        }
    }
}
