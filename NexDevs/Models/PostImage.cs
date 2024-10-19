using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class PostImage
    {
        [Key]
        public int PostId { get; set; }

        public int WorkId { get; set; }

        public string ContentPost { get; set; }

        public IFormFile PostImageUrl { get; set; }

        public DateTime CreateAt { get; set; }

        public int LikesCount { get; set; }

        public int CommentsCount { get; set; }

        public int Approved { get; set; }
    }
}
