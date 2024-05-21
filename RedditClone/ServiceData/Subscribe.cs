using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceData
{
    public class Subscribe : TableEntity
    {
        private string userId;
        private string postId;

        public string UserId { get => userId; set => userId = value; }
        public string PostId { get => postId; set => postId = value; }

        public Subscribe() { }
       

        public Subscribe(string userId, string postId)
        {
            PartitionKey = "Subscribe";
            RowKey = PostId;

            this.userId = userId;          
        }
    }
}
