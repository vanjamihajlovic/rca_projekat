using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Helpers.JWT
{
    public class JwtTokenReader
    {
        public string ExtractTokenFromAuthorizationHeader(System.Net.Http.Headers.AuthenticationHeaderValue authorizationHeader)
        {
            if (authorizationHeader == null)
            {
                return null;
            }

            string authHeaderString = authorizationHeader.ToString(); 

            if (string.IsNullOrWhiteSpace(authHeaderString))
                return null;

            string[] parts = authHeaderString.Split(' ');
            if (parts.Length != 2 || parts[0] != "Bearer")
                return null;

            return parts[1];
        }

        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims;
        }

        public string GetClaimValue(IEnumerable<Claim> claims, string claimType)
        {
            var claim = claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }

    }
}
