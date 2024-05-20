﻿using Helpers;
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
    [RoutePrefix("post")]
	public class PostController : ApiController
    {
        TableRepositoryTema repo = new TableRepositoryTema();
        TableRepositoryKomentar repoKom = new TableRepositoryKomentar();

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

                // Pronađi temu po Id-u
                Tema temaToDelete = await Task.FromResult(repo.DobaviTemu(id));
                if (temaToDelete == null)
                {                 
                    return BadRequest("Failed to delete post.");
                }
                List<Komentar> svi = repoKom.DobaviSve().ToList();
                var p = svi.Where(x => x.IdTeme == temaToDelete.Id).ToList();

                temaToDelete.Komentari = p.Select(x => x.RowKey).ToList();


                // Briši komentare povezane sa temom
                if (temaToDelete.Komentari != null)
                {
                    // Briši komentare povezane sa temom
                    foreach (string komentarId in temaToDelete.Komentari)
                    {
                        bool isCommentDeleted = await Task.FromResult(repoKom.ObrisiKomentar(komentarId));
                        if (!isCommentDeleted)
                        {
                            // Ukoliko nije uspeo da obrise komentar, vrati false
                            return BadRequest("Failed to delete comment.");
                        }
                    }
                }

                // Delete post using repository
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