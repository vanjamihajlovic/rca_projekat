using Helpers.JWT;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RedditService_WebRole.Models;
using ServiceData;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;


namespace RedditService_WebRole.Controllers
{
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
        private CloudTable korisnikTable;
        private JwtToken JwtToken;

        public AuthController()
        {
            try
            {
                // Preuzmite konekcioni string iz konfiguracije
                var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
                var tableClient = storageAccount.CreateCloudTableClient();
                korisnikTable = tableClient.GetTableReference("Korisnici");
                korisnikTable.CreateIfNotExists();
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

        public async Task<IHttpActionResult> Login(Login data)
        {
            try
            {
                if (ModelState.IsValid == false || data == null)
                {
                    return BadRequest();
                }

                try
                {
                    var retrieveOperation = TableOperation.Retrieve<Korisnik>("Korisnik", data.Email);
                    var result = await korisnikTable.ExecuteAsync(retrieveOperation);

                    var korisnik = result.Result as Korisnik;
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