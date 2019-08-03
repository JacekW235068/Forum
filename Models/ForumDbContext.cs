using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class ForumDbContext : IdentityDbContext<AppUser>
    {
        public ForumDbContext(DbContextOptions options)
              : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SubForum> SubForums { get; set; }
        public DbSet<ForumThread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
