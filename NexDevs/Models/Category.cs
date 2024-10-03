using System.ComponentModel.DataAnnotations;

namespace NexDevs.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryImageUrl { get; set; }
    }
}

