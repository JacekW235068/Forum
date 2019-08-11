using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    //TODO: change model to contain foreign kye
    public class RefreshToken
    {
        [Key]
        public Guid TokenId { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public AppUser user { get; set; }
    }
}
