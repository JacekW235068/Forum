using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.DTO
{
    public class RefreshTokenPost
    {
        public string refreshToken { get; set; }
        public string accessToken { get; set; }
    }
}
