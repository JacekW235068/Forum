using Forum.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Forum.Services
{
    public interface ITokenFactory
    {
        string StandardAccessToken(IdentityUser user, IList<string> roles);
        RefreshToken StandardRefreshToken();
       
    }
}
