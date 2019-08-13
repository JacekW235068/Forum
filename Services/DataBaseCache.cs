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
        private List<SubForumGet> subForums;
        private List<ForumThread> threads;
        private readonly int maxThreads;
        public IReadOnlyCollection<ForumThread> Threads { get { return threads.AsReadOnly(); } }
        public IReadOnlyCollection<SubForumGet> SubForums { get { return subForums.AsReadOnly(); } }
       
        public DataBaseCache(int maxItems)
        {
            maxThreads = maxItems;
            subForums = new List<SubForumGet>();
        }
              
        public void Init(ForumDbContext context)
        {
            if (threads == null)
                threads = context.Threads.OrderByDescending(x => x.LastPostTime).Take(maxThreads).ToList();
           
        }
        public void AddThread(ForumThread thread)
        {
            if (thread.ThreadID == null ||
                thread.Title == null ||
                thread.Text == null)
                throw new NullReferenceException();

            var i = threads.Select(x => x.ThreadID).ToList().IndexOf(thread.ThreadID);
            if(threads.Count == maxThreads)
                if (i == -1)
                    threads.RemoveAt(maxThreads - 1);
                else
                    threads.RemoveAt(i);
            threads.Add(thread);
        }
        public bool DeleteThread(string Id)
        {
            var i = threads.Select(x => x.ThreadID.ToString()).ToList().IndexOf(Id);
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
