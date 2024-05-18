﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditService_WebRole.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }  // Vreme kada je post kreiran

        // Lista komentara - pretpostavka da je lista sa ID-jevima komentara
        public List<int> Comments { get; set; }

        // Lista korisnika koji su pretplaćeni - pretpostavka da je lista sa ID-jevima korisnika
        public List<int> SubscribedUsers { get; set; }

        // Broj glasova
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }

        public Post()
        {
            Comments = new List<int>();
            SubscribedUsers = new List<int>();
        }
    }
}