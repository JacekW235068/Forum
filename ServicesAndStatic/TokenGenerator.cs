using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Services
{
    public class TokenGenerator: ITokenFactory
    {
        private readonly string Key;
        private readonly double ExpirationDate;
        private readonly string Issuer;
        private readonly double RefreshExpirationDate;

        public TokenGenerator(string key, double expirationDate, string issuer, double refreshExpirationDate)
        {
            Key = key;
            ExpirationDate = expirationDate;
            Issuer = issuer;
           
            RefreshExpirationDate = refreshExpirationDate;
        }

      

        public string StandardAccessToken(IdentityUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            foreach( var r in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(1);

            var token = new JwtSecurityToken(
                Issuer,
                Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken StandardRefreshToken()
        {
            var randomNumber = new byte[8];
            string token; 
            using (var rng = RandomNumberGenerator.Create())
            {
                    rng.GetBytes(randomNumber);
                    token = Convert.ToBase64String(randomNumber);
            }
            return new RefreshToken()
            {
                Token = token,
                ExpirationDate = DateTime.Now.AddDays(RefreshExpirationDate)
            };
        }

     
    }
}
