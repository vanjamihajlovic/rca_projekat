using Helpers;
using Microsoft.WindowsAzure.Storage.Queue;
using RedditService_WebRole.Models;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TableRepository;

namespace RedditService_WebRole.Controllers
{
    [RoutePrefix("vote")]
    public class VoteController : ApiController
    {
        TableRepositoryVote repo = new TableRepositoryVote();

        [HttpPost]
        [Route("upvote")]
        public async Task<IHttpActionResult> Upvote(Voting voteModel)
        {
            if (voteModel == null || string.IsNullOrEmpty(voteModel.UserId) || string.IsNullOrEmpty(voteModel.PostId))
            {
                return BadRequest("Invalid vote data.");
            }

            try
            {
                // Provera da li je korisnik već upvotovao ovaj post
                var existingVotes = repo.DobaviSveGlasoveZaKorisnika(voteModel.UserId);
                bool hasVoted = existingVotes.Any(v => v.PostId == voteModel.PostId);
                if (hasVoted)
                {
                    return BadRequest("User has already upvoted this post.");
                }

                var vote = new Vote(Guid.NewGuid().ToString(), voteModel.UserId, voteModel.PostId, true, DateTime.UtcNow);

                bool isAdded = repo.DodajGlas(vote);
                if (!isAdded)
                {
                    return BadRequest("Failed to upvote.");
                }

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
        public async Task<IHttpActionResult> Downvote(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid vote ID.");
                }

                // Delete comment using repository
                bool isDeleted = await Task.FromResult(repo.ObrisiGlas(id));
                if (!isDeleted)
                {
                    return BadRequest("Failed to delete vote.");
                }

                return Ok("Vote deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting
                Console.WriteLine($"Exception occurred in vote: {ex.Message}");
                return InternalServerError(ex);
            }
        }
    }
    
}