using Helpers;
using Helpers.JWT;
using Microsoft.WindowsAzure.Storage.Queue;
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
    [RoutePrefix("vote")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VoteController : ApiController
    {
        private readonly TableRepositoryVote _repo;
        private readonly JwtTokenReader _jwtTokenReader;

        public VoteController()
        {
            _repo = new TableRepositoryVote();
            _jwtTokenReader = new JwtTokenReader();
        }

        [HttpPost]
        [Route("upvote/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<IHttpActionResult> Upvote(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid vote ID.");

            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);
                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                var existingVotes = _repo.DobaviSveGlasoveZaKorisnika(emailClaim);
                bool hasVoted = existingVotes.Any(v => v.PostId == id);
                if (hasVoted)
                    return BadRequest("User has already upvoted this post.");

                var vote = new Vote(Guid.NewGuid().ToString(), emailClaim, id, true, DateTime.UtcNow);
                bool isAdded = _repo.DodajGlas(vote);
                if (!isAdded)
                    return BadRequest("Failed to upvote.");

                await QueueHelper.GetQueueReference("VoteNotificationsQueue").AddMessageAsync(new CloudQueueMessage(vote.RowKey));
                return Ok("Upvote successful and notification sent.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("downvote/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<IHttpActionResult> Downvote(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid vote ID.");

            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                bool isDeleted = await Task.FromResult(_repo.ObrisiGlas(id));
                if (!isDeleted)
                    return BadRequest("Failed to delete vote.");

                return Ok("Vote deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}