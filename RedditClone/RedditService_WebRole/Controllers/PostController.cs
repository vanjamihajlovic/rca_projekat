﻿using Helpers;
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
    [RoutePrefix("post")]
    public class PostController : ApiController
    {
        TableRepositoryTema repo = new TableRepositoryTema();

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> PostPost(Post post)
        {
            try
            {
                if (ModelState.IsValid == false || post == null)
                {
                    return BadRequest();
                }

                var noviPost = new Tema(post.Id, post.Title, post.Content);

                // Dodavanje posta korišćenjem servisa
                bool isAdded = await Task.FromResult(repo.DodajTemu(noviPost));
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

        [HttpPost]
        [Route("delete/{id}")]
        public async Task<IHttpActionResult> DeletePost(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid post ID.");
                }

                // Delete comment using repository
                bool isDeleted = await Task.FromResult(repo.ObrisiTemu(id));
                if (!isDeleted)
                {
                    return BadRequest("Failed to delete post.");
                }

                return Ok("Post deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details for troubleshooting          
                return InternalServerError(ex);
            }
        }

    }
}