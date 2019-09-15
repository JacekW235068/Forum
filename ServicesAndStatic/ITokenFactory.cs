using Forum.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Forum.Services
{
    public interface ITokenFactory
    {
        /// <returns>JWT access token for user and his roles</returns>
        string StandardAccessToken(IdentityUser user, IList<string> roles);
        /// <summary>
        /// Generate Token valid for amount of days defined in configuration file
        /// </summary>
        /// <returns>Refresh Token in form of random chars</returns>
        RefreshToken StandardRefreshToken();
       
    }
}
