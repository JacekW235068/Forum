using Forum.Models;
using Forum.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    public interface IDatabaseCache
    {
        IReadOnlyCollection<ForumThreadGet> Threads { get; }
        IReadOnlyCollection<SubForumGet> SubForums { get; }
        /// <summary>
        /// Gets maximum number of threads stored in cache
        /// </summary>
        int MaxThreads { get; }
        /// <summary>
        /// Add thread to most popular threads
        /// </summary>
        void AddThread(ForumThread thread);
        /// <summary>
        /// Update thread in cache
        /// </summary>
        /// <param name="postCount"> Amount of post to add to a counter</param>
        /// <returns>Returns true if thread exists in cache and was updated</returns>
        bool UpdateThread(string threadID, int postCount);
        /// <summary>
        /// Delete thread from cache
        /// </summary>
        /// <returns>Returns true if thread was removed</returns>
        bool DeleteThread(ForumThread thread);
        /// <summary>
        /// Fetch Subforums from database
        /// </summary>
        /// <param name="context">Context of database used for fetching</param>
        void RefreshSubForums(ForumDbContext context);
        /// <param name="context">Database context</param>
        /// <returns>threads fetched from cache/database</returns>
        IReadOnlyCollection<ForumThreadGet> GetThreads(ForumDbContext context, uint start, uint amount);
        void MoveThread(string from, string to);


    }
}
