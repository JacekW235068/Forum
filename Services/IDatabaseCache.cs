using Forum.Models;
using Forum.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    public interface IDatabaseCache
    {
        IReadOnlyCollection<ForumThread> Threads { get; }
        IReadOnlyCollection<SubForumGet> SubForums { get; }
        void AddThread(ForumThread thread);
        bool DeleteThread(string Id);
        void RefreshSubForums(ForumDbContext context);
        
    }
}
