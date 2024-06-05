using Helpers.JWT;
using RedditService_WebRole.Models;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using TableRepository;


namespace RedditService_WebRole.Controllers
{
    [RoutePrefix("user")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly TableRepositoryKorisnik _userRepository;
        private readonly JwtTokenReader _jwtTokenReader;

        public UserController()
        {
            _userRepository = new TableRepositoryKorisnik();
            _jwtTokenReader = new JwtTokenReader();
        }


        [HttpPut]
        [Route("profile/update")]
        public async Task<IHttpActionResult> UpdateUserProfile([FromBody] User updatedUser)
        {
            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);
                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                if (string.IsNullOrEmpty(emailClaim) || emailClaim != updatedUser.Email)
                    return BadRequest("Email mismatch or invalid token.");

                Korisnik user = await Task.FromResult(_userRepository.DobaviKorisnika(emailClaim));
                if (user == null)
                    return NotFound();

                if (updatedUser != null)
                {
                    user.Ime = updatedUser.FirstName ?? user.Ime;
                    user.Prezime = updatedUser.LastName ?? user.Prezime;
                    user.Adresa = updatedUser.Address ?? user.Adresa;
                    user.Grad = updatedUser.City ?? user.Grad;
                    user.Drzava = updatedUser.Country ?? user.Drzava;
                    user.BrTel = updatedUser.Phone ?? user.BrTel;
                    user.Lozinka = updatedUser.Password ?? user.Lozinka;
                    user.Slika = updatedUser.Image ?? user.Slika;

                    bool isUpdated = await Task.FromResult(_userRepository.IzmeniKorisnika(user.RowKey, user));
                    if (!isUpdated)
                        return BadRequest("Failed to update user profile.");
                }

                    // Update user properties
                    

                return Ok("User profile updated successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

}
