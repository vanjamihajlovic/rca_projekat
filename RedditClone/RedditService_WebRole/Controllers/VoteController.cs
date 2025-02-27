﻿using Helpers;
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
        private readonly TableRepositoryTema _repositoryTema;
        private readonly JwtTokenReader _jwtTokenReader;

        public VoteController()
        {
            _repo = new TableRepositoryVote();
            _jwtTokenReader = new JwtTokenReader();
            _repositoryTema = new TableRepositoryTema();
        }

        [HttpPost]
        [Route("upvote/{postId}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<IHttpActionResult> Upvote(string postId)
        {
            if (string.IsNullOrEmpty(postId))
                return BadRequest("Invalid vote ID.");

            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);
                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                Vote userVote = _repo.DobaviGlasovaNaPostZaKorisnik(emailClaim, postId);

                // Use case when user already voted. Now the vote gets 'de-voted' 
                // If the deleted vote is a upvote, we add a downvote, if it was an downvote we just remove it
                if (userVote != null)
                {
                    bool isDeleted = await _repo.ObrisiGlasAsync(userVote.VoteId);

                    if (userVote.IsUpvote) {
                        bool decremented = await _repositoryTema.UpdateVoteCount(postId, false, true);
                        return Ok();
                    }
                    else
                    {
                        bool decremented = await _repositoryTema.UpdateVoteCount(postId, false, false);
                    }
                }

                var vote = new Vote(Guid.NewGuid().ToString(), emailClaim, postId, true, DateTime.UtcNow);
                bool isAdded = _repo.DodajGlas(vote);
                bool incremented = await _repositoryTema.UpdateVoteCount(postId, true, true);

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
        [Route("downvote/{postId}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<IHttpActionResult> Downvote(string postId)
        {
            if (string.IsNullOrEmpty(postId))
                return BadRequest("Invalid vote ID.");

            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);
                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                Vote userVote = _repo.DobaviGlasovaNaPostZaKorisnik(emailClaim, postId);

                // Use case when user already voted. Now the vote gets 'de-voted' 
                // If the deleted vote is a upvote, we add a downvote, if it was an downvote we just remove it
                if (userVote != null)
                {

                    bool isDeleted = await _repo.ObrisiGlasAsync(userVote.VoteId);

                    if (!userVote.IsUpvote)
                    {
                        bool decremented = await _repositoryTema.UpdateVoteCount(postId, false, false);
                        return Ok();
                    }
                    else
                    {
                        bool decremented = await _repositoryTema.UpdateVoteCount(postId, false, true);
                    }
                }

                var vote = new Vote(Guid.NewGuid().ToString(), emailClaim, postId, false, DateTime.UtcNow);
                bool isAdded = _repo.DodajGlas(vote);
                bool incremented = await _repositoryTema.UpdateVoteCount(postId, true, false);

                if (!isAdded)
                    return BadRequest("Failed to upvote.");

                return Ok("Downvote successful.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
