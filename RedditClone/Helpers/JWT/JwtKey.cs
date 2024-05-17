using Microsoft.IdentityModel.Tokens;
using System.Text;


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
