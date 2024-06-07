using Contracts;
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
    [RoutePrefix("comment")]
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class CommentController : ApiController
    {
        private readonly JwtTokenReader _jwtTokenReader = new JwtTokenReader();
        private readonly TableRepositoryKomentar _commentRepository = new TableRepositoryKomentar();

        // GET: Comment
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> PostComment(Comment comment)
        {
            if (comment == null || ModelState.IsValid == false)
            {
                return BadRequest("Comment cannot be empty.");
            }
            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);

                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");
                var firstName = _jwtTokenReader.GetClaimValue(claims, "firstName");
                var lastName = _jwtTokenReader.GetClaimValue(claims, "lastName");
                var noviKomentar = new Komentar(comment.TopicId, emailClaim, comment.Text, firstName + " " + lastName);

                bool isAdded = _commentRepository.DodajKomentar(noviKomentar);
                if (!isAdded)
                {
                    return BadRequest();
                }

                await QueueHelper.GetQueueReference("CommentNotificationsQueue").AddMessageAsync(new CloudQueueMessage(noviKomentar.RowKey));

                return Ok(noviKomentar);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("delete/{id}")]
        public async Task<IHttpActionResult> DeleteComment(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid comment ID.");
                }
                                      
                bool isDeleted = await Task.FromResult(_commentRepository.ObrisiKomentar(id));
                if (!isDeleted)
                {
                    return BadRequest("Failed to delete comment.");
                }

                return Ok("Comment deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred in DeleteComment: {ex.Message}");
                return InternalServerError(ex);
            }
        }
    }
}
