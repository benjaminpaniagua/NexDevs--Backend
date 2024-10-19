using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public int PostId { get; set; }

        public int? UserId { get; set; }

        public int? WorkId { get; set; }

        public string ContentComment { get; set; }

        public DateTime CreateAt { get; set; }

        public int LikesCount { get; set; }
    }
}
