using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TableRepository;

namespace RedditService_WebRole.Controllers
{
	[RoutePrefix("vote")]
	public class VoteController : ApiController
	{
		TableRepositoryVote repo = new TableRepositoryVote();
	}
}