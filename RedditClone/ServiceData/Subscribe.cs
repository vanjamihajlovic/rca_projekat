using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ServiceData
{
    public class Subscribe : TableEntity
    {
        public string UserId { get; set; }
        public string PostId { get; set; }

        public Subscribe()
        {
            PartitionKey = "Subscribe";
        }

        public Subscribe(string userId, string postId) : this()
        {
            RowKey = postId;
            UserId = userId;
        }
    }
}
