﻿using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public int WorkId { get; set; }

        public string ContentPost { get; set; }
        public int PaymentReceipt { get; set; }

        public string PostImageUrl { get; set; }

        public DateTime CreateAt { get; set; }

        public int LikesCount { get; set; }

        public int CommentsCount { get; set; }

        public int Approved { get; set; }
    }
}
