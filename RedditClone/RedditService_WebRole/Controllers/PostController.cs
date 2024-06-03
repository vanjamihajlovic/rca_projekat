using Contracts;
using Helpers;
using Helpers.JWT;
using RedditService_WebRole.Models;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using TableRepository;

namespace RedditService_WebRole.Controllers
{

    [RoutePrefix("post")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PostController : ApiController
    {
        private readonly TableRepositoryTema _repo;
        private readonly TableRepositoryKorisnik _repoKorisnik;
        private readonly TableRepositoryKomentar _repoKommentar;
        private readonly JwtTokenReader _jwtTokenReader;

        public PostController()
        {
            _repo = new TableRepositoryTema();
            _repoKorisnik = new TableRepositoryKorisnik();
            _repoKommentar = new TableRepositoryKomentar();
            _jwtTokenReader = new JwtTokenReader();
        }

        [HttpPost]
        [Route("create")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<IHttpActionResult> CreatePost(Post post)
        {
            try
            {
                if (!ModelState.IsValid || post == null)
                    return BadRequest();

                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);

                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");
                var firstName = _jwtTokenReader.GetClaimValue(claims, "firstName");
                var lastName = _jwtTokenReader.GetClaimValue(claims, "lastName");

                var newPost = new Tema(post.Id, post.Title, post.Content, emailClaim, firstName, lastName);
                bool isAdded = await Task.FromResult(_repo.DodajTemu(newPost));

                if (!isAdded)
                    return BadRequest();

                return Ok("Post created successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        
        [HttpPost]
        [Route("delete/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]

        public async Task<IHttpActionResult> DeletePost(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid post ID.");
                }

                Tema temaToDelete = await Task.FromResult(_repo.DobaviTemu(id));
                if (temaToDelete == null)
                {                 
                    return BadRequest("Failed to delete post.");
                }
                List<Komentar> svi = _repoKommentar.DobaviSve().ToList();

                temaToDelete.Komentari = svi.Where(x => x.IdTeme == temaToDelete.Id).ToList();


                if (temaToDelete.Komentari != null)
                {
                    foreach (string komentarId in temaToDelete.Komentari.Select(x => x.RowKey).ToList())
                    {
                        bool isCommentDeleted = await Task.FromResult(_repoKommentar.ObrisiKomentar(komentarId));
                        if (!isCommentDeleted)
                        {
                            return BadRequest("Failed to delete comment.");
                        }
                    }
                }

                // Delete post using repository
                bool isDeleted = await Task.FromResult(_repo.ObrisiTemu(id));
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

        [HttpGet]
        [Route("search")]
        public async Task<IHttpActionResult> SearchPosts(string searchTerm)
        {
            try
            {
                // Validacija parametara pretrage
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return BadRequest("Search term is required.");
                }

                // Pretraživanje tema po naslovu
                var results = await Task.FromResult(_repo.PretraziTeme(searchTerm));

                // Pretvorba rezultata u listu i vraćanje HTTP odgovora
                var searchResults = results.ToList();
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("sort")]
        public IHttpActionResult SortPosts(string sortBy = "Title", string sortOrder = "asc")
        {
            try
            {
                // Preuzmi sve teme iz repozitorijuma
                var allPosts = _repo.DobaviSve();

                // Sortiranje rezultata
                switch (sortBy.ToLower())
                {
                    case "title":
                        allPosts = sortOrder.ToLower() == "desc" ? allPosts.OrderByDescending(t => t.Naslov) : allPosts.OrderBy(t => t.Naslov);
                        break;
                    // Dodati dodatne case-ove ako želite omogućiti sortiranje po drugim poljima
                    default:
                        return BadRequest("Invalid sort by parameter.");
                }

                // Pretvorba rezultata u listu i vraćanje HTTP odgovora
                var sortedResults = allPosts.ToList();
                return Ok(sortedResults);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("read/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]

        public async Task<IHttpActionResult> ReadPost(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid post ID.");
                }

                Tema post = await Task.FromResult(_repo.DobaviTemu(id));
                List<Komentar> svi = _repoKommentar.DobaviSve().ToList();

                post.Komentari = svi.Where(x => x.IdTeme == post.Id).ToList();
                if (post == null)
                {
                    return NotFound();
                }

                return Ok(post);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("readall")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<IHttpActionResult> ReadAllPosts()
        {
            try
            {
                var allPosts = await Task.FromResult(_repo.DobaviSve().ToList());
                return Ok(allPosts);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}