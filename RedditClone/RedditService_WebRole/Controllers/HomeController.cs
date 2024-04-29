using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/*
Ako ćemo stvarno raditi sa React-om, onda ovo nasleđuje ApiController
Iznad metoda se postavlja:
	[HttpPost]
	[ActionName("imeAkcije")]
	[Authorize] (ako zahteva autorizaciju)
	[AllowAnonymus] (ako može bilo ko da pristupi)
*/

namespace RedditService_WebRole.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
