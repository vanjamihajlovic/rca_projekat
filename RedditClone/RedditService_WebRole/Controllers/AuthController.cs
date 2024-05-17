using Helpers.JWT;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RedditService_WebRole.Models;
using ServiceData;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using TableRepository;

namespace RedditService_WebRole.Controllers
{
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
        private JwtToken JwtToken;
        TableRepositoryKorisnik repo = new TableRepositoryKorisnik();

        public AuthController()
        {
            try
            {
                JwtToken = new JwtToken("8673298319820138das980dsadsa2131", "CDL", "CLD");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }
        // GET: Auth
        [HttpPost]
        [Route("login")]

        public IHttpActionResult Login(Login data)
        {
            try
            {
                if (ModelState.IsValid == false || data == null)
                {
                    return BadRequest();
                }

                try
                {
                    var korisnik = repo.DobaviKorisnika(data.Email);
                    if (korisnik != null && korisnik.Lozinka == data.Password)
                    {
                        var token = JwtToken.GenerateToken(data.Email);
                        return Ok(token);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }


            }
            catch
            {
                return BadRequest();
            }
        }

        // register

    }
}