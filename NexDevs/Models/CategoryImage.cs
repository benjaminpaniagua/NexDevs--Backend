using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class CategoryImage
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public IFormFile CategoryImageUrl { get; set; }
    }
}

