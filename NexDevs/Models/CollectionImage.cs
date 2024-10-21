using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class CollectionImage
    {
        [Key]
        public int CollectionId { get; set; }

        public int WorkId { get; set; }

        public IFormFile CollectionImageUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}
