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
using System.Security.Cryptography;
using System.Text;
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
                    // Hash the password with the salt
                    string hashedPassword = HashPassword(data.Password);

                    var korisnik = repo.DobaviKorisnika(data.Email);
                    if (korisnik != null && korisnik.Lozinka == hashedPassword)
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
                    // Hash the password with the salt
                    string hashedPassword = HashPassword(data.Password);

                    var novi = new Korisnik(data.Email, data.FirstName, data.LastName, data.Address, data.City, data.Country, data.Phone, data.Email, hashedPassword);

                    if (data.Image != "")
                    {
                        Image image;
                        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(data.Image.Split(',')[1])))
                        {
                            image = Image.FromStream(ms);
                        }
                        novi.Slika = new BlobHelper().UploadImage(image, "slike",
                            Guid.NewGuid().ToString() + ".jpg");
                    } else
                    {
                        novi.Slika = "";
                    }
                    

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

        #region  Hash Helpers
        static string HashPassword(string password)
        {
            byte[] salt = Encoding.UTF8.GetBytes("CGYzqeN4plZekNC88Umm1Q==");
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                // Concatenate password and salt
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Hash the concatenated password and salt
                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Concatenate the salt and hashed password for storage
                byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
                Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
                Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

                return Convert.ToBase64String(hashedPasswordWithSalt);
            }
        }
        #endregion
    }
}