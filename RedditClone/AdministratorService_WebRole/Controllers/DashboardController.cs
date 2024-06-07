using AdministratorService_WebRole.Models;
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
    [RoutePrefix("dashboard")]
    public class DashboardController : ApiController
    {
        TableRepositoryIzvestaj repo = new TableRepositoryIzvestaj();
        
        [HttpGet]
        [Route("getHour")]
        public async Task<IHttpActionResult> GetPreviousHour()
        {
            try
            {
                var previousHour = await Task.FromResult(repo.DobaviIzvestajeZaPrethodniSat());
                return Ok(previousHour);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("getDay")]
        public async Task<IHttpActionResult> GetPreviousDay()
        {
            try
            {
                var previousDay = await Task.FromResult(repo.DobaviProsekZaPrethodniDan());
                return Ok(previousDay);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
