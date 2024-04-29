using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

//Klasa koja je odgovorna za generisanje JWT tokena koji se može koristiti za autentifikaciju i autorizaciju korisnika u aplikaciji
//Claims su informacije o korisniku ili entitetu koji će biti enkodirane u tokenu, kao što je korisničko ime i uloge

namespace Helpers.JWT
{
    public class JwtToken
    {
        private readonly JwtKey _key;
        private readonly string _issuer; //Izdavač tokena, tj. entitet koji je generisao token
        private readonly string _audience; //Auditorijum tokena, tj. entitet koji je namenjen da ga koristi

        public JwtToken(string secretKey, string issuer, string audience)
        {
            _key = new JwtKey(secretKey);
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(string username, string[] roles, int expiryMinutes = 60)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _key.GetKey();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, string.Join(",", roles))
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

