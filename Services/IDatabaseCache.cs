using Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    public interface IDatabaseCache
    {
        IReadOnlyCollection<ForumThread> Threads { get; }
        void Add(ForumThread thread);
        //prevents user from calling init (but at what cost tho)
        //void Init(ForumDbContext context);
    }
}
