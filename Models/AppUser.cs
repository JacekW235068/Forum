﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class AppUser : IdentityUser
    {
        public List<RefreshToken> RefreshTokens { get; set; }
        public List<Post> Posts { get; set; }

        internal object Include()
        {
            throw new NotImplementedException();
        }
    }
}
