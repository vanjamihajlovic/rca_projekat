using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ServiceData
{
    public class Vote : TableEntity
    {
        public string VoteId { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public bool IsUpvote { get; set; }
        public DateTime VotedAt { get; set; }

        public Vote()
        {
            PartitionKey = "Vote";
        }

        public Vote(string voteId, string userId, string postId, bool isUpvote, DateTime votedAt) : this()
        {
            RowKey = voteId;
            VoteId = voteId;
            UserId = userId;
            PostId = postId;
            IsUpvote = isUpvote;
            VotedAt = votedAt;
        }
    }
}
