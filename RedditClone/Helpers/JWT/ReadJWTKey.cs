using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.JWT
{
    public class ReadJWTKey
    {
        public IEnumerable<System.Security.Claims.Claim> GetJWTClaimsFromKey(string token) { 
        
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.ReadJwtToken(token).Claims;

        }
    }
}
