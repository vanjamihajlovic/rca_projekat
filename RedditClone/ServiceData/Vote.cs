using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace ServiceData
{
    public class Vote : TableEntity
    {
        private string voteId;
        private string userId;       // ID korisnika koji je glasao
        private string postId;       // ID teme 
        private bool isUpvote;       // Da li je glas upvote (true) ili downvote (false)
        private DateTime votedAt;    // Datum i vreme glasanja

        public string VoteId { get => voteId; set => voteId = value; }
        public string UserId { get => userId; set => userId = value; }
        public string PostId { get => postId; set => postId = value; }
        public bool IsUpvote { get => isUpvote; set => isUpvote = value; }
        public DateTime VotedAt { get => votedAt; set => votedAt = value; }

        public Vote() { }

        public Vote(string voteId, string userId, string postId, bool isUpvote, DateTime votedAt)
        {
            PartitionKey = "Subscribe";
            RowKey = Guid.NewGuid().ToString();

            VoteId = voteId;
            UserId = userId;
            PostId = postId;
            IsUpvote = isUpvote;
            VotedAt = votedAt;
        }
    }
}
