using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Helpers.JWT
{
    public class JwtKey
    {
        private readonly string _secretKey;

        public JwtKey(string secretKey)
        {
            _secretKey = secretKey;
        }

        public SymmetricSecurityKey GetKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        }
    }
}
