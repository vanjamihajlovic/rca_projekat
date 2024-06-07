using Contracts;
using Helpers;
using Helpers.JWT;
using RedditService_WebRole.Models;
using ServiceData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Security;
using System.Web.Services.Description;
using TableRepository;
using System.Drawing;


namespace RedditService_WebRole.Controllers
{

    [RoutePrefix("post")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PostController : ApiController
    {
        private readonly TableRepositoryTema _postRepository;
        private readonly TableRepositoryKorisnik _userRepository;
        private readonly TableRepositoryKomentar _commentRepository;
        private readonly TableRepositoryVote _voteRepository;
        private readonly TableRepositorySubscribe _subscriptionRepository;
        private readonly JwtTokenReader _jwtTokenReader;

        public PostController()
        {
            _postRepository = new TableRepositoryTema();
            _userRepository = new TableRepositoryKorisnik();
            _commentRepository = new TableRepositoryKomentar();
            _voteRepository = new TableRepositoryVote();
            _subscriptionRepository = new TableRepositorySubscribe();
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

                    if (post.ImageUrl != "")
                {
                    Image image;
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(post.ImageUrl.Split(',')[1])))
                    {
                        image = Image.FromStream(ms);
                    }
                    newPost.Slika = new BlobHelper().UploadImage(image, "slike",
                            Guid.NewGuid().ToString() + ".jpg");
                
                } else
                {
                    newPost.Slika = "";
                }
                bool isAdded = await Task.FromResult(_postRepository.DodajTemu(newPost));

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

                Tema temaToDelete = await Task.FromResult(_postRepository.DobaviTemu(id));
                if (temaToDelete == null)
                {                 
                    return BadRequest("Failed to delete post.");
                }
                List<Komentar> svi = _commentRepository.DobaviSve().ToList();

                temaToDelete.Komentari = svi.Where(x => x.IdTeme == temaToDelete.Id).ToList();


                if (temaToDelete.Komentari != null)
                {
                    foreach (string komentarId in temaToDelete.Komentari.Select(x => x.RowKey).ToList())
                    {
                        bool isCommentDeleted = await Task.FromResult(_commentRepository.ObrisiKomentar(komentarId));
                        if (!isCommentDeleted)
                        {
                            return BadRequest("Failed to delete comment.");
                        }
                    }
                }

                // Delete post using repository
                bool isDeleted = await Task.FromResult(_postRepository.ObrisiTemu(id));
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
                var results = await Task.FromResult(_postRepository.PretraziTeme(searchTerm));

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
                var allPosts = _postRepository.DobaviSve();

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

                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);

                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                Tema post = await Task.FromResult(_postRepository.DobaviTemu(id));
                List<Komentar> svi = await Task.FromResult(_commentRepository.DobaviSve().ToList());
                List<Vote> votes = await Task.FromResult(_voteRepository.DobaviSveGlasoveZaPost(id));
                List<Subscribe> subscribes = await Task.FromResult(_subscriptionRepository.DobaviSvePrijavljene(id));

                post.Komentari = svi.Where(x => x.IdTeme == post.Id).ToList();
                if (post == null)
                {
                    return NotFound();
                }

                if (emailClaim == post.UserId)
                {
                    post.IsOwner = true;
                }

                if (subscribes.Any(subscribe => subscribe.UserId == emailClaim))
                {
                    post.IsSubscribed = true;
                } 

                votes.ForEach(vote =>
                {
                    

                    if (vote.UserId == emailClaim)
                    {
                        if (vote.IsUpvote)
                        {
                            post.PostVoteStatus = "UPVOTED";
                        }
                        else if (!vote.IsUpvote) {
                            post.PostVoteStatus = "DOWNVOTED";
                        }
                    }

                    if (vote.IsUpvote)
                    {
                        post.GlasoviZa++;

                    } else if (!vote.IsUpvote)
                    {
                        post.GlasoviProtiv++;
                    }
                });

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
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);

                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                var allPosts = await Task.FromResult(_postRepository.DobaviSve().ToList());
                List<Vote> votes = await Task.FromResult(_voteRepository.DobaviSve().ToList());
                List<Subscribe> subscribes = await Task.FromResult(_subscriptionRepository.DobaviSve().ToList());


                foreach (var post in allPosts)
                {
                    if (emailClaim == post.UserId)
                    {
                        post.IsOwner = true;
                    }

                    if (subscribes.Any(subscribe => subscribe.UserId == emailClaim && subscribe.PostId == post.Id))
                    {
                        post.IsSubscribed = true;
                    }

                    votes.Where(vote => vote.PostId == post.Id).ToList().ForEach(vote =>
                    {

                        if (vote.UserId == emailClaim)
                        {
                            if (vote.IsUpvote)
                            {
                                post.PostVoteStatus = "UPVOTED";
                            }
                            else if (!vote.IsUpvote)
                            {
                                post.PostVoteStatus = "DOWNVOTED";
                            }
                        }

                        if (vote.IsUpvote)
                        {
                            post.GlasoviZa++;

                        }
                        else if (!vote.IsUpvote)
                        {
                            post.GlasoviProtiv++;
                        }
                    });
                }
                

                return Ok(allPosts);


            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        
        [HttpGet]
        [Route("readallpaginated")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<IHttpActionResult> ReadAllPostsPaginated(int page = 1, int pageSize = 5, string sortBy = "date")
        {
            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);
                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                var paginatedPostsTask = _postRepository.DobaviSvePaginirano(page, pageSize, sortBy);
                var paginatedPosts = await paginatedPostsTask;

                var votes = await Task.FromResult(_voteRepository.DobaviSve().ToList());
                var subscribes = await Task.FromResult(_subscriptionRepository.DobaviSve().ToList());

                foreach (var post in paginatedPosts)
                {
                    if (emailClaim == post.UserId)
                    {
                        post.IsOwner = true;
                    }

                    if (subscribes.Any(subscribe => subscribe.UserId == emailClaim && subscribe.PostId == post.Id))
                    {
                        post.IsSubscribed = true;
                    }

                    votes.Where(vote => vote.PostId == post.Id).ToList().ForEach(vote =>
                    {
                        if (vote.UserId == emailClaim)
                        {
                            if (vote.IsUpvote)
                            {
                                post.PostVoteStatus = "UPVOTED";
                            }
                            else if (!vote.IsUpvote)
                            {
                                post.PostVoteStatus = "DOWNVOTED";
                            }
                        }

                    });
                }

                return Ok(paginatedPosts);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }


}