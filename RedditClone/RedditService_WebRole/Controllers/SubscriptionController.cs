using Helpers.JWT;
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
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class SubscriptionController : ApiController
    {
        TableRepositorySubscribe repo = new TableRepositorySubscribe();
        private readonly JwtTokenReader _jwtTokenReader = new JwtTokenReader();


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
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);

                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                var noviSub = new Subscribe(emailClaim, request.PostId);

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