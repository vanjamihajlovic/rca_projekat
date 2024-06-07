using AdministratorService_WebRole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

// WEB 2 API - Empty

namespace AdministratorService_WebRole.Controllers
{
	[RoutePrefix("login")]
	public class LoginController : ApiController
    {
		[HttpPost]
		[Route("signin")]
		public IHttpActionResult SignIn([FromBody] User loginData)
		{
			if (loginData.Username == "admin" && loginData.Password == "admin")
			{
				return Ok("Uspesna prijava");
			}

			return BadRequest("Pogresno korisnicko ime i/ili lozinka.");
		}

		[HttpPost]
		[Route("signout")]
		public IHttpActionResult SignOut()
		{
			try
			{
				HttpContext.Current.Session.Abandon();
				return Ok();
			}
			catch
			{
				return InternalServerError();
			}
		}
	}
}
