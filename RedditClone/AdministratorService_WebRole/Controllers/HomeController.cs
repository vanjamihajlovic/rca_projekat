using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace AdministratorService_WebRole.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            return Redirect(Request.RequestUri.AbsoluteUri + "Views/Index.html");
        }
    }
}
