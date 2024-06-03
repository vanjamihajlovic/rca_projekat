using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

//Klasa koja je odgovorna za generisanje JWT tokena koji se može koristiti za autentifikaciju i autorizaciju korisnika u aplikaciji
//Claims su informacije o korisniku ili entitetu koji će biti enkodirane u tokenu, kao što je korisničko ime i uloge

namespace Helpers.JWT
{
    public class JwtToken
    {
        private static JwtKey _key;
        private static string _issuer; //Izdavač tokena, tj. entitet koji je generisao token
        private static string _audience; //Auditorijum tokena, tj. entitet koji je namenjen da ga koristi

        public JwtToken(string secretKey, string issuer, string audience)
        {
            _key = new JwtKey(secretKey);
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(string email, string firstName, string lastName, int expiryMinutes = 60)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _key.GetKey();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim("firstName", firstName),
                    new Claim("lastName", lastName),
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

