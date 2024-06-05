using Helpers;
using Helpers.JWT;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RedditService_WebRole.Models;
using ServiceData;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
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
                        var token = JwtToken.GenerateToken(data.Email, korisnik.Ime, korisnik.Prezime);
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // register
        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(Register data)
        {
            try
            {
                if(ModelState.IsValid == false || data == null)
                {
                    return BadRequest();
                }

                var korisnik = repo.DobaviKorisnika(data.Email);
                if (korisnik == null || korisnik.Id == null)
                {
                    var novi = new Korisnik(data.Email, data.FirstName, data.LastName, data.Address, data.City, data.Country, data.Phone, data.Email, data.Password);

                    Image image;
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(data.Image.Split(',')[1])))
                    {
                        image = Image.FromStream(ms);
                    }
                    novi.Slika = new BlobHelper().UploadImage(image, "slike",
                        Guid.NewGuid().ToString() + ".jpg");

                    bool result = repo.DodajKorisnika(novi);

                    if(result)
                    {
                        var token = JwtToken.GenerateToken(data.Email, data.FirstName, data.LastName);
                        return Ok(token);
                    }
                    else
                    {
                        return BadRequest();
                    }
                    
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
      // implementirati logout na frontu tako da izbaci cookie iz storage-a
    }
}