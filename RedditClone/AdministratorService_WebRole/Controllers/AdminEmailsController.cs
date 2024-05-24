using AdministratorService_WebRole.Models;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TableRepository;

namespace AdministratorService_WebRole.Controllers
{
    [RoutePrefix("emails")]
    public class AdminEmailsController : ApiController
    {
        TableRepositoryAdminEmail repo = new TableRepositoryAdminEmail();

        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAllEmails()
        {
            try
            {
                // TODO autorizacija
                //if (HttpContext.Current.Session["User"] == null)
                //{
                //    return BadRequest("Niste ulogovani!");
                //}

                //User usr = HttpContext.Current.Session["User"] as User;
                //if (usr.Username != "admin")
                //{
                //    return BadRequest("Vi niste admin!");
                //}

                var allEmails = await Task.FromResult(repo.DobaviSveMejlove());
                return Ok(allEmails);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddEmail(Email adresa)
        {
            try
            {
                if (ModelState.IsValid == false || adresa == null)
                {
                    return BadRequest();
                }

                var noviMejl = new AdminEmail(adresa.EmailAddress);

                // Dodavanje posta korišćenjem servisa
                bool isAdded = await Task.FromResult(repo.DodajMejlAdresu(noviMejl));
                if (!isAdded)
                {
                    return BadRequest("That email address is allready added.");
                }

                return Ok("New Admin email added successfully.");

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IHttpActionResult> DeleteEmail(Email adresa)
        {
            try
            {
                if (ModelState.IsValid == false || adresa == null)
                {
                    return BadRequest();
                }

                // Delete post using repository
                bool isDeleted = await Task.FromResult(repo.ObrisiMejlAdresu(adresa.EmailAddress));
                if (!isDeleted)
                {
                    return BadRequest("Failed to delete email address.");
                }

                return Ok("Email address deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting          
                return InternalServerError(ex);
            }
        }
    }
}
