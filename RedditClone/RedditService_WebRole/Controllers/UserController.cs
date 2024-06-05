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
        public async Task<IHttpActionResult> UpdateUserProfile([FromBody]Korisnik updatedUser)
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
                    user.Ime = updatedUser.Ime ?? user.Ime;
                    user.Prezime = updatedUser.Prezime ?? user.Prezime;
                    user.Adresa = updatedUser.Adresa ?? user.Adresa;
                    user.Grad = updatedUser.Grad ?? user.Grad;
                    user.Drzava = updatedUser.Drzava?? user.Drzava;
                    user.BrTel = updatedUser.BrTel ?? user.BrTel;
                    user.Lozinka = updatedUser.Lozinka ?? user.Lozinka;
                    user.Slika = updatedUser.Slika ?? user.Slika;

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


        [HttpGet]
        [Route("get")]
        public async Task<IHttpActionResult> GetUser()
        {
            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);
                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                if (string.IsNullOrEmpty(emailClaim))
                    return BadRequest("No Email Claim");

                Korisnik user = await Task.FromResult(_userRepository.DobaviKorisnika(emailClaim));

                if (user == null)
                    return NotFound();

                user.Lozinka = "";

                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }

}
