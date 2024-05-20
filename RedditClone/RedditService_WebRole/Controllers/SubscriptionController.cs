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
    [RoutePrefix("subscribe")]
	public class SubscriptionController : ApiController
    {
        TableRepositorySubscribe repo = new TableRepositorySubscribe();

        [HttpPost]
        [Route("subscribepost")]
        public async Task<IHttpActionResult> SubscribeToPost([FromBody] SubscriptionRequest request)
        {
            try
            {
                if (ModelState.IsValid == false || request == null)
                {
                    return BadRequest();
                }

                var noviSub = new Subscribe(request.UserId, request.PostId);

                // Dodavanje posta korišćenjem servisa
                bool isAdded = await Task.FromResult(repo.SubscribeToPost(noviSub));
                if (!isAdded)
                {
                    return BadRequest();
                }

                return Ok("Post posted and notifications sent successfully.");

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        
        

    }
}