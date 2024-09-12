using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class Collection
    {
        [Key]
        public int CollectionId { get; set; }

        public int WorkId { get; set; }

        public string CollectionImageUrl { get; set; }
    }
}
